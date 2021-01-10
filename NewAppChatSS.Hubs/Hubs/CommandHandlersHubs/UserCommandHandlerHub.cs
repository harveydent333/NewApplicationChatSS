﻿using System;
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
    public class UserCommandHandlerHub : IUserCommandHandler
    {
        private readonly Dictionary<Regex, Func<User, string, Task>> userCommands;

        private readonly UserManager<User> userManager;
        private readonly IUserValidator userValidator;

        private IHubCallerClients clients;

        public UserCommandHandlerHub(UserManager<User> userManager, IUserValidator userValidator)
        {
            userCommands = new Dictionary<Regex, Func<User, string, Task>>
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
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommandAsync(User user, string command, IHubCallerClients clients)
        {
            this.clients = clients;

            foreach (Regex keyCommand in userCommands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    return userCommands[keyCommand](user, command);
                }
            }

            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.IncorrectCommand));
        }

        /// <summary>
        /// Метод изменяет старый логин пользователя на новый
        /// </summary>
        private async Task<Task> UserRenameAsync(User user, string command)
        {
            string ownerUserName = user.UserName;

            Dictionary<string, string> userNames = ParsingUserNames(command);

            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, true, userNames["oldUserName"]))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
            }

            user = await userManager.FindByNameAsync(userNames["oldUserName"]);

            if (await userManager.FindByNameAsync(userNames["oldUserName"]) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
            }

            if (await userManager.FindByNameAsync(userNames["newUserName"]) == null)
            {
                user.UserName = userNames["newUserName"];
                await userManager.UpdateAsync(user);

                if (ownerUserName == userNames["oldUserName"])
                {
                    return clients.Caller.SendAsync(
                        "UserRenameClient",
                        userNames["newUserName"],
                        CommandHandler.CreateCommandInfo(InformationMessages.UserNameHasBeenChanged));
                }
                else
                {
                    return clients.Caller.SendAsync(
                        "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNameHasBeenChanged));
                }
            }

            return clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNameIsAlreadyInUse));
        }

        /// <summary>
        /// Метод бессрочно блокирует пользоваетля в приложении
        /// </summary>
        private async Task<Task> UserBanAsync(User user, string command)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
            }

            UserName = Regex.Replace(command, @"^//user\sban\s", string.Empty);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
            }

            UserName = await ChangedStatusBlockingUserAsync(UserName, command, true, true);

            return clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenBlocked));
        }

        /// <summary>
        /// Метод разблокироует пользователя в приложении
        /// </summary>
        private async Task<Task> UserPardonAsync(User user, string command)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
            }

            UserName = Regex.Replace(command, @"^//user\spardon\s", string.Empty);

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
            }

            UserName = await ChangedStatusBlockingUserAsync(UserName, command, false);

            return clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenUnblocked));
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        private async Task<Task> TemporaryUserBlockAsync(User user, string command)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
            }

            UserName = Regex.Match(command, @"//user\sban\s(.+)\s-m\s\d*$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
            }

            UserName = await ChangedStatusBlockingUserAsync(UserName, command, true);

            return clients.Caller.SendAsync(
                "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserHasBeenBlocked));
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        private async Task<Task> SetModerationRoleAsync(User user, string command)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
            }

            UserName = Regex.Match(command, @"//user\smoderator\s(.+)\s-n$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
            }

            user = await userManager.FindByNameAsync(UserName);
            await userManager.AddToRoleAsync(user, "Moderator");

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {UserName} был назначен модератором."));
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        private async Task<Task> RemoveModerationRoleAsync(User user, string command)
        {
            if (!await userValidator.CommandAccessCheckAsync(user, new List<string> { "Administrator", "Moderator" }, false, ""))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.AccessIsDenied));
            }

            UserName = Regex.Match(command, @"//user\smoderator\s(.+)\s-d$").Groups[1].Value;

            if (await userManager.FindByNameAsync(UserName) == null)
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.UserNotFound));
            }

            user = await userManager.FindByNameAsync(UserName);
            await userManager.RemoveFromRoleAsync(user, "Moderator");

            return clients.Caller.SendAsync(
                "ReceiveCommand",
                CommandHandler.CreateCommandInfo($"Пользователь {UserName} был разжалован до обычного пользователя."));
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
