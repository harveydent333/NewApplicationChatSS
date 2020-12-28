using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Constants;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с пользователем
    /// </summary>
    public class UserCommandHandlerHub : Hub, IUserCommandHandler
    {
        private static string _userName;

        private readonly Dictionary<Regex, Func<User, string, IHubCallerClients, Task>> userCommands;

        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;

        public UserCommandHandlerHub(UserManager<User> userManager, IUserValidator userValidator)
        {
            userCommands = new Dictionary<Regex, Func<User, string, IHubCallerClients, Task>>
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

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommandAsync(User user, string command, IHubCallerClients calledClients)
        {
            foreach (Regex keyCommand in userCommands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    return userCommands[keyCommand](user, command, calledClients);
                }
            }

            return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
        }

        /// <summary>
        /// Метод изменяет старый логин пользователя на новый
        /// </summary>
        private async Task<Task> UserRenameAsync(User user, string command, IHubCallerClients clients)
        {
            string ownerUserName = user.UserName;

            Dictionary<string, string> userNames = ParsingUserNames(command);

            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, true, userNames["oldUserName"]))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            user = await userManager.FindByNameAsync(userNames["oldUserName"]);

            if (await userManager.FindByNameAsync(userNames["newUserName"]) == null)
            {
                if (await userManager.FindByNameAsync(userNames["oldUserName"]) == null)
                {
                    return clients.Caller.SendAsync(
                        "ReceiveCommand",
                        CommandHandler.CreateCommandInfo($"Пользователь с именем {userNames["oldUserName"]} не найден."));
                }

                user.UserName = userNames["newUserName"];
                await userManager.UpdateAsync(user);

                if (ownerUserName == userNames["oldUserName"])
                {
                    return clients.Caller.SendAsync(
                        "UserRenameClient",
                        userNames["newUserName"],
                        CommandHandler.CreateCommandInfo($"Имя было успешно изменено на {userNames["newUserName"]}"));
                }
                else
                {
                    return clients.Caller.SendAsync(
                        "ReceiveCommand",
                        CommandHandler.CreateCommandInfo($"Имя пользователя {userNames["oldUserName"]} было успешно изменен " +
                            $"на {userNames["newUserName"]}"));
                }
            }

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Имя пользователя {userNames["oldUserName"]} уже занято"));
        }

        /// <summary>
        /// Метод бессрочно блокирует пользоваетля в приложении
        /// </summary>
        private async Task<Task> UserBanAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            _userName = Regex.Replace(command, @"^//user\sban\s", string.Empty);

            if (await userManager.FindByNameAsync(_userName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь с именем {_userName} не найден."));
            }

            _userName = await ChangedStatusBlockingUserAsync(_userName, command, true, true);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {_userName} заблокирован."));
        }

        /// <summary>
        /// Метод разблокироует пользователя в приложении
        /// </summary>
        private async Task<Task> UserPardonAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            _userName = Regex.Replace(command, @"^//user\spardon\s", string.Empty);

            if (await userManager.FindByNameAsync(_userName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь с именем {_userName} не найден."));
            }

            _userName = await ChangedStatusBlockingUserAsync(_userName, command, false);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {_userName} разблокирован."));
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        private async Task<Task> TemporaryUserBlockAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            _userName = Regex.Match(command, @"//user\sban\s(.+)\s-m\s\d*$").Groups[1].Value;

            if (await userManager.FindByNameAsync(_userName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь с именем {_userName} не найден."));
            }

            _userName = await ChangedStatusBlockingUserAsync(_userName, command, true);

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {_userName} заблокирован."));
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        private async Task<Task> SetModerationRoleAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            _userName = Regex.Match(command, @"//user\smoderator\s(.+)\s-n$").Groups[1].Value;

            if (await userManager.FindByNameAsync(_userName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь с именем {_userName} не найден."));
            }

            user = await userManager.FindByNameAsync(_userName);
            await userManager.AddToRoleAsync(user, "Moderator");

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {_userName} был назначен модератором."));
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        private async Task<Task> RemoveModerationRoleAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            _userName = Regex.Match(command, @"//user\smoderator\s(.+)\s-d$").Groups[1].Value;

            if (await userManager.FindByNameAsync(_userName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь с именем {_userName} не найден."));
            }

            user = await userManager.FindByNameAsync(_userName);
            await userManager.RemoveFromRoleAsync(user, "Moderator");

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {_userName} был разжалован до обычного пользователя."));
        }

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

        /// <summary>
        /// Метод меняет статус роли пользователя
        /// </summary>
        private async Task<string> ChangedStatusBlockingUserAsync(string userName, string command, bool blockStatus, bool isIndefiniteBlock = false)
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
