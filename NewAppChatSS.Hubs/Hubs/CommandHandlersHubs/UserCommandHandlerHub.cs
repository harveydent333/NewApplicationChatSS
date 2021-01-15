using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.Hubs.Hubs.CommandHandlersHubs;
using NewAppChatSS.Hubs.Infrastructure;
using NewAppChatSS.Hubs.Interfaces.HubInterfaces;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с пользователем
    /// </summary>
    public class UserCommandHandlerHub : AbstarctHub, IUserCommandHandler
    {
        public override Dictionary<Regex, Func<string, Task>> Commands { get; }

        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;

        public UserCommandHandlerHub(UserManager<User> userManager, IUserValidator userValidator)
        {
            Commands = new Dictionary<Regex, Func<string, Task>>
            {
                [new Regex(@"^//user\srename\s([0-9A-z])+\w\W{2}\w([0-9A-z])+\S$")] = UserRenameAsync,
                [new Regex(@"^//user\sban\s([0-9A-z])+\S$")] = UserBanAsync,
                [new Regex(@"^//user\sban\s([0-9A-z])+\s-m\s\d*$")] = TemporaryUserBlockAsync,
                [new Regex(@"^//user\spardon\s([0-9A-z])+\S$")] = UserPardonAsync,
                [new Regex(@"^//user\smoderator\s([0-9A-z])+\s-n$")] = SetModerationRoleAsync,
                [new Regex(@"^//user\smoderator\s([0-9A-z])+\s-d$")] = RemoveModerationRoleAsync,
            };

            this.userManager = userManager;
            this.userValidator = userValidator;
        }

        private static string UserName { get; set; }

        /// <summary>
        /// Метод изменяет старый логин пользователя на новый
        /// </summary>
        private async Task UserRenameAsync(string command)
        {
            string ownerUserName = user.UserName;

            Dictionary<string, string> userNames = ParsingUserNames(command);

            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(user, acceptableRoles, true, userNames["oldUserName"]))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
                return;
            }

            user = await userManager.FindByNameAsync(userNames["oldUserName"]);

            if (await userManager.FindByNameAsync(userNames["oldUserName"]) == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
                return;
            }

            if (await userManager.FindByNameAsync(userNames["newUserName"]) == null)
            {
                user.UserName = userNames["newUserName"];
                await userManager.UpdateAsync(user);

                if (ownerUserName == userNames["oldUserName"])
                {
                    await clients.Caller.SendAsync(
                        "UserRenameClient",
                        userNames["newUserName"],
                        CommandHandler.CreateCommandInfo(InformationMessages.UserNameHasBeenChanged));

                    return;
                }
                else
                {
                    await clients.Caller.SendAsync(
                        "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNameHasBeenChanged));

                    return;
                }
            }

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNameIsAlreadyInUse));
        }

        /// <summary>
        /// Метод бессрочно блокирует пользоваетля в приложении
        /// </summary>
        private async Task UserBanAsync(string command)
        {
            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(user, acceptableRoles, false, ""))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));

                return;
            }

            UserName = Regex.Replace(command, @"^//user\sban\s", string.Empty);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));

                return;
            }

            UserName = await ChangedStatusBlockingUserAsync(UserName, command, true, true);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenBlocked));
        }

        /// <summary>
        /// Метод разблокироует пользователя в приложении
        /// </summary>
        private async Task UserPardonAsync(string command)
        {
            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(user, acceptableRoles, false, ""))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));

                return;
            }

            UserName = Regex.Replace(command, @"^//user\spardon\s", string.Empty);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));

                return;
            }

            UserName = await ChangedStatusBlockingUserAsync(UserName, command, false, false);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenUnblocked));
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        private async Task TemporaryUserBlockAsync(string command)
        {
            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(user, acceptableRoles, false, ""))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));

                return;
            }

            UserName = Regex.Match(command, @"//user\sban\s(.+)\s-m\s\d*$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));

                return;
            }

            UserName = await ChangedStatusBlockingUserAsync(UserName, command, true, false);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenBlocked));
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        private async Task SetModerationRoleAsync(string command)
        {
            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(user, acceptableRoles, false, ""))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));

                return;
            }

            UserName = Regex.Match(command, @"//user\smoderator\s(.+)\s-n$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));

                return;
            }

            user = await userManager.FindByNameAsync(UserName);
            await userManager.AddToRoleAsync(user, RoleConstants.ModeratorRole);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo($"Пользователь {UserName} был назначен модератором."));
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        private async Task RemoveModerationRoleAsync(string command)
        {
            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(user, acceptableRoles, false, ""))
            {
                await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));

                return;
            }

            UserName = Regex.Match(command, @"//user\smoderator\s(.+)\s-d$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));

                return;
            }

            user = await userManager.FindByNameAsync(UserName);
            await userManager.RemoveFromRoleAsync(user, RoleConstants.ModeratorRole);

            await clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo($"Пользователь {UserName} был разжалован до обычного пользователя."));
        }

        // TODO: Вынести отсюда

        /// <summary>
        /// Метод получает старый и новый логин из текста переданной команды
        /// </summary>
        private Dictionary<string, string> ParsingUserNames(string stringNames)
        {
            return new Dictionary<string, string>
            {
                ["oldUserName"] = Regex.Match(stringNames, @"//user rename (.+\w)\W{2}(\w.+)$").Groups[1].Value,
                ["newUserName"] = Regex.Match(stringNames, @"//user rename (.+\w)\W{2}(\w.+)$").Groups[2].Value
            };
        }

        // TODO: Вынести отсюда

        /// <summary>
        /// Метод меняет статус роли пользователя
        /// </summary>
        private async Task<string> ChangedStatusBlockingUserAsync(string userName, string command, bool blockStatus, bool isIndefiniteBlock)
        {
            User user = await userManager.FindByNameAsync(userName);
            user.IsLocked = blockStatus;

            if (isIndefiniteBlock)
            {
                user.DateUnblock = DateTime.Now.AddYears(GlobalConstants.IndefininiteBlocking);
            }
            else
            {
                user.DateUnblock = TimeComputer.CalculateUnlockDate(command, @"//user\sban\s.+\s-m\s(\d*)$");
            }

            await userManager.UpdateAsync(user);

            return user.UserName;
        }
    }
}
