using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Entities;
using AutoMapper;
using System.Linq;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.BLL.Infrastructure;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;

namespace NewAppChatSS.BLL.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с пользователем
    /// </summary>
    public class UserCommandHandlerHub : Hub, IUserCommandHandler
    {
        private readonly Dictionary<Regex, Func<User, string, IHubCallerClients, Task>> userCommands;
        //    const Int32 ROLE_REGULAR_USER = 1;
        //    const Int32 ROLE_MODERATOR = 2;
        const int INDEFINITE_BLOCKING = 100;

        private static string userName;


        private IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserValidator _userValidator;

        public UserCommandHandlerHub(UserManager<User> userManager, IMapper mapper, IUserValidator userValidator, IUnitOfWork uow)
        {
            userCommands = new Dictionary<Regex, Func<User, string, IHubCallerClients, Task>>
            {
                [new Regex(@"^//user\srename\s\w+\W\W\w+\S$")] = UserRenameAsync,
                [new Regex(@"^//user\sban\s\w+\S$")] = UserBanAsync,
                [new Regex(@"^//user\sban\s\w+\s-m\s\d*$")] = TemporaryUserBlockAsync,
                [new Regex(@"^//user\spardon\s\w+\S$")] = UserPardonAsync,
                [new Regex(@"^//user\smoderator\s\w+\s-n$")] = SetModerationRoleAsync,
                [new Regex(@"^//user\smoderator\s\w+\s-d$")] = RemoveModerationRoleAsync,
            };

            Database = uow;
            _userManager = userManager;
            _mapper = mapper;
            _userValidator = userValidator;
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
            //return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
            return calledClients.Caller.SendAsync("ReceiveCommand", "Неверная команда");
        }

        /// <summary>
        /// Метод изменяет старый логин пользователя на новый
        /// </summary>
        public async Task<Task> UserRenameAsync(User user, string command, IHubCallerClients clients)
        {
            string ownerUserName = user.UserName;

            Dictionary<string, string> userNames = ParsingUserNames(command);

            if (!await _userValidator.CommandAccessCheckAsync(user,
                new string[] { "Administrator", "Moderator" }, true, userNames["oldUserName"]))
            {

                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            user = await _userManager.FindByNameAsync(userNames["oldUserName"]);

            if (await _userManager.FindByNameAsync(userNames["newUserName"]) == null)
            {
                if (await _userManager.FindByNameAsync(userNames["oldUserName"]) == null)
                {
                    //return clients.Caller.SendAsync("ReceiveCommand",
                    //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userNames["oldUserName"]} не найден."));
                    return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userNames["oldUserName"]} не найден.");
                }

                user.UserName = userNames["newUserName"];
                await _userManager.UpdateAsync(user);


                if (ownerUserName == userNames["oldUserName"])
                {
                    //return clients.Caller.SendAsync("UserRenameClient", userNames["newUserName"],
                    //    CommandHandler.CreateCommandInfo($"Имя было успешно изменено на {userNames["newUserName"]}"));
                    return clients.Caller.SendAsync("ReceiveCommand", $"Имя было успешно изменено на {userNames["newUserName"]}");
                }
                else
                    //return clients.Caller.SendAsync("ReceiveCommand",
                    //    CommandHandler.CreateCommandInfo($"Имя пользователя {userNames["oldUserName"]} было успешно изменен " +
                    //        $"на {userLogins["newLogin"]}"));
                    return clients.Caller.SendAsync("ReceiveCommand",
                        $"Имя пользователя {userNames["oldUserName"]} было успешно изменено на {userNames["newUserName"]}");
            }

            //return clients.Caller.SendAsync("ReceiveCommand",
            //  CommandHandler.CreateCommandInfo($"Имя пользователя {userLogins["oldLogin"]} уже занято"));
            return clients.Caller.SendAsync("ReceiveCommand", $"Имя пользователя {userNames["oldUserName"]} уже занято");

        }

        /// <summary>
        /// Метод бессрочно блокирует пользоваетля в приложении
        /// </summary>
        public async Task<Task> UserBanAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await _userValidator.CommandAccessCheckAsync(user, new string[] { "Administrator", "Moderator" }))
            {
                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            userName = Regex.Replace(command, @"^//user\sban\s", string.Empty);

            if (await _userManager.FindByNameAsync(userName) == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userName} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userName} не найден.");
            }

            userName = await ChangedStatusBlockingUserAsync(userName, command, true, true);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {userName} заблокирован."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userName} заблокирован.");
        }

        /// <summary>
        /// Метод разблокироует пользователя в приложении
        /// </summary>
        public async Task<Task> UserPardonAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await _userValidator.CommandAccessCheckAsync(user, new string[] { "Administrator", "Moderator" }))
            {
                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            userName = Regex.Replace(command, @"^//user\spardon\s", string.Empty);

            if (await _userManager.FindByNameAsync(userName) == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userName} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userName} не найден.");
            }

            userName = await ChangedStatusBlockingUserAsync(userName, command, false);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {userName} разблокирован."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userName} разблокирован.");
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        public async Task<Task> TemporaryUserBlockAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await _userValidator.CommandAccessCheckAsync(user, new string[] { "Administrator", "Moderator" }))
            {
                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            userName = Regex.Match(command, @"//user\sban\s(\w+)\s-m\s\d*$").Groups[1].Value;

            if (await _userManager.FindByNameAsync(userName) == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userName} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userName} не найден.");
            }

            userName = await ChangedStatusBlockingUserAsync(userName, command, true);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {userName} заблокирован."));
            return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь {userName} заблокирован.");
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        public async Task<Task> SetModerationRoleAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await _userValidator.CommandAccessCheckAsync(user, new string[] { "Administrator", "Moderator" }))
            {
                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            userName = Regex.Match(command, @"//user\smoderator\s(\w+)\s-n$").Groups[1].Value;

            if (await _userManager.FindByNameAsync(userName) == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userName} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userName} не найден.");
            }

            user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, "Moderator");

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {user.Login} был назначен модератором."));
            return clients.Caller.SendAsync("ReceiveCommand", "");
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        public async Task<Task> RemoveModerationRoleAsync(User user, string command, IHubCallerClients clients)
        {
            if (!await _userValidator.CommandAccessCheckAsync(user, new string[] { "Administrator", "Moderator" }))
            {
                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            userName = Regex.Match(command, @"//user\smoderator\s(\w+)\s-d$").Groups[1].Value;

            if (await _userManager.FindByNameAsync(userName) == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userName} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userName} не найден.");
            }

            user = await _userManager.FindByNameAsync(userName);
            await _userManager.RemoveFromRoleAsync(user, "Moderator");

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {user.Login} был разжалован до обычного пользователя."));
            return clients.Caller.SendAsync("ReceiveCommand", "");
        }

        /// <summary>
        /// Метод получает старый и новый логин из текста переданной команды
        /// </summary>
        public Dictionary<string, string> ParsingUserNames(string stringNames)
        {
            return new Dictionary<string, string>
            {
                ["oldUserName"] = Regex.Match(stringNames, @"//user rename (\w+)\W\W(\w+)$").Groups[1].Value,
                ["newUserName"] = Regex.Match(stringNames, @"//user rename (\w+)\W\W(\w+)$").Groups[2].Value
            };
        }

        /// <summary>
        /// Метод меняет статус роли пользователя
        /// </summary>
        public async Task<string> ChangedStatusBlockingUserAsync(string userName, string command, bool blockStatus, bool isIndefiniteBlock = false)
        {
            User user = await _userManager.FindByNameAsync(userName);
            user.IsLocked = blockStatus;

            if (isIndefiniteBlock)
            {
                user.DateUnblock = DateTime.Now.AddYears(INDEFINITE_BLOCKING);
            }
            else
            {
                user.DateUnblock = TimeComputer.CalculateUnlockDate(command, @"//user\sban\s\w+\s-m\s(\d*)$");
            }

            await _userManager.UpdateAsync(user);

            return user.UserName;
        }
    }
}
