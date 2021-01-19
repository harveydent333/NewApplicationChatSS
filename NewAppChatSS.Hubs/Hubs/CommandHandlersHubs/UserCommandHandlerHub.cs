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
            string ownerUserName = currentUser.UserName;

            Dictionary<string, string> userNames = ParsingUserNames(command);

            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };

            if (!await userValidator.CommandAccessCheckAsync(currentUser, acceptableRoles, true, userNames["oldUserName"]))
            {
                await SendResponseMessage(ValidationMessageConstants.AccessIsDenied);
                return;
            }

            currentUser = await userManager.FindByNameAsync(userNames["oldUserName"]);

            if (await userManager.FindByNameAsync(userNames["oldUserName"]) == null)
            {
                await SendResponseMessage(ValidationMessageConstants.UserNotFound);
                return;
            }

            if (await userManager.FindByNameAsync(userNames["newUserName"]) == null)
            {
                currentUser.UserName = userNames["newUserName"];
                await userManager.UpdateAsync(currentUser);

                if (ownerUserName == userNames["oldUserName"])
                {
                    await clients.Caller.SendAsync(
                        "UserRenameClient",
                        userNames["newUserName"],
                        CommandHandler.CreateResponseMessage(InformationMessageConstants.UserNameHasBeenChanged));

                    return;
                }
                else
                {
                    await SendResponseMessage(InformationMessageConstants.UserNameHasBeenChanged);
                    return;
                }
            }

            await SendResponseMessage(ValidationMessageConstants.UserNameAlreadyInUse);
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

            if (!await userValidator.CommandAccessCheckAsync(currentUser, acceptableRoles, false, ""))
            {
                await SendResponseMessage(ValidationMessageConstants.AccessIsDenied);
                return;
            }

            UserName = Regex.Replace(command, @"^//user\sban\s", string.Empty);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await SendResponseMessage(ValidationMessageConstants.UserNotFound);

                return;
            }

            await ChangedStatusBlockingUserAsync(UserName, command, true, true);

            await SendResponseMessage(InformationMessageConstants.UserHasBeenBlocked);
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

            if (!await userValidator.CommandAccessCheckAsync(currentUser, acceptableRoles, false, ""))
            {
                await SendResponseMessage(ValidationMessageConstants.AccessIsDenied);
                return;
            }

            UserName = Regex.Replace(command, @"^//user\spardon\s", string.Empty);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await SendResponseMessage(ValidationMessageConstants.UserNotFound);
                return;
            }

            await ChangedStatusBlockingUserAsync(UserName, command, false, false);

            await SendResponseMessage(InformationMessageConstants.UserHasBeenUnblocked);
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        private async Task TemporaryUserBlockAsync(string command)
        {
            // какой вариант с точки зрения правильности код стайла лучше
/*
            var acceptableRoles = new List<string>
            {
                RoleConstants.AdministratorRole,
                RoleConstants.ModeratorRole
            };
*/
            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await userValidator.CommandAccessCheckAsync(currentUser, acceptableRoles, false, ""))
            {
                await SendResponseMessage(ValidationMessageConstants.AccessIsDenied);
                return;
            }

            UserName = Regex.Match(command, @"//user\sban\s(.+)\s-m\s\d*$").Groups[1].Value;

            // получить в переменную var processedUser and check in method Validato IsNullUser
            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await SendResponseMessage(ValidationMessageConstants.UserNotFound);
                return;
            }

            await ChangedStatusBlockingUserAsync(UserName, command, true, false);

            await SendResponseMessage(InformationMessageConstants.UserHasBeenBlocked);
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        private async Task SetModerationRoleAsync(string command)
        {
            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await userValidator.CommandAccessCheckAsync(currentUser, acceptableRoles, false, ""))
            {
                await SendResponseMessage(ValidationMessageConstants.AccessIsDenied);
                return;
            }

            UserName = Regex.Match(command, @"//user\smoderator\s(.+)\s-n$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await SendResponseMessage(ValidationMessageConstants.UserNotFound);
                return;
            }

            currentUser = await userManager.FindByNameAsync(UserName);
            await userManager.AddToRoleAsync(currentUser, RoleConstants.ModeratorRole);

            await SendResponseMessage(InformationMessageConstants.UserAssignedModerRole);
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        private async Task RemoveModerationRoleAsync(string command)
        {
            var acceptableRoles = new List<string> { RoleConstants.AdministratorRole, RoleConstants.ModeratorRole };

            if (!await userValidator.CommandAccessCheckAsync(currentUser, acceptableRoles, false, ""))
            {
                await SendResponseMessage(ValidationMessageConstants.AccessIsDenied);
                return;
            }

            UserName = Regex.Match(command, @"//user\smoderator\s(.+)\s-d$").Groups[1].Value;

            var user = await userManager.FindByNameAsync(UserName);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                await SendResponseMessage(ValidationMessageConstants.UserNotFound);
                return;
            }

            await userManager.RemoveFromRoleAsync(currentUser, RoleConstants.ModeratorRole);

            await SendResponseMessage(InformationMessageConstants.UserAssignedRegularRole);
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
        private async Task ChangedStatusBlockingUserAsync(string userName, string command, bool blockStatus, bool isIndefiniteBlock)
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
        }
    }
}
