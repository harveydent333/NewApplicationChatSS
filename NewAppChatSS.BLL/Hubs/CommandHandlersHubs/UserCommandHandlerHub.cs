using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.BLL.DTO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Entities;
using AutoMapper;
using System.Linq;
using NewAppChatSS.BLL.Interfaces;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с пользователем
    /// </summary>
    public class UserCommandHandlerHub : Hub, IUserCommandHandler
    {
        private readonly Dictionary<Regex, Func<UserDTO, string, IHubCallerClients, Task>> userCommands;
        //    const Int32 ROLE_REGULAR_USER = 1;
        //    const Int32 ROLE_MODERATOR = 2;
        //    const Int32 INDEFINITE_BLOCKING = 100;

        private static string userLogin;


        private IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserValidator _userValidator;

        public UserCommandHandlerHub(UserManager<User> userManager, IMapper mapper, IUserValidator userValidator)
        {
            userCommands = new Dictionary<Regex, Func<UserDTO, string, IHubCallerClients, Task>>
            {
                [new Regex(@"^//user\srename\s\w+\W\W\w+\S$")] = UserRename,
                [new Regex(@"^//user\sban\s\w+\S$")] = UserBan,
                [new Regex(@"^//user\sban\s\w+\s-m\s\d*$")] = TemporaryUserBlock,
                [new Regex(@"^//user\spardon\s\w+\S$")] = UserPardon,
                [new Regex(@"^//user\smoderator\s\w+\s-n$")] = SetModerationRole,
                [new Regex(@"^//user\smoderator\s\w+\s-d$")] = RemoveModerationRole,
            };

            _userManager = userManager;
            _mapper = mapper;
            _userValidator = userValidator;
        }

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommand(UserDTO user, string command, IHubCallerClients calledClients)
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
        public async Task<Task> UserRename(UserDTO userDto, string command, IHubCallerClients clients)
        {
            string ownerLogin = userDto.UserName;

            Dictionary<String, String> userNames = ParsingUserNames(command);

            if (!await _userValidator.CommandAccessCheckAsync(userDto, new String[] { "Administrator", "Moderator" }, true, userNames["oldUserName"]))
            {

                //return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
                return clients.Caller.SendAsync("ReceiveCommand", "Отказано в доступе.");
            }

            User user = await _userManager.FindByNameAsync(userNames["oldUserName"]);
            userDto = _mapper.Map<UserDTO>(user);

            if (_userManager.FindByNameAsync(userNames["oldUserName"]) == null)
            {
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Пользователь с именем {userNames["oldUserName"]} не найден."));
                return clients.Caller.SendAsync("ReceiveCommand", $"Пользователь с именем {userNames["oldUserName"]} не найден.");
            }

            userDto.UserName = userNames["newUserName"];
            //user = _mapper.Map<User>(userDto);
            await _userManager.UpdateAsync(_mapper.Map<User>(userDto));
            Database.Save();

            if (ownerLogin == userNames["oldUserName"])
            {
                //return clients.Caller.SendAsync("UserRenameClient", userNames["newUserName"],
                //    CommandHandler.CreateCommandInfo($"Имя было успешно изменено на {userNames["newUserName"]}"));
                return clients.Caller.SendAsync("ReceiveCommand", $"Имя было успешно изменено на {userNames["newUserName"]}");
            }
            else
                //return clients.Caller.SendAsync("ReceiveCommand",
                //    CommandHandler.CreateCommandInfo($"Имя пользователя {userNames["oldUserName"]} было успешно изменен " +
                //        $"на {userLogins["newLogin"]}"));
                return clients.Caller.SendAsync("ReceiveCommand", $"Имя пользователя {userNames["oldUserName"]} было успешно изменен " +
                    $"на {userNames["newUserName"]}");
        }

        /// <summary>
        /// Метод бессрочно блокирует пользоваетля в приложении
        /// </summary>
        public Task UserBan(UserDTO user, string command, IHubCallerClients clients)
        {
            //        if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }))
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            //        }
            //        userLogin = Regex.Replace(command, @"^//user\sban\s", String.Empty);

            //        if (userRepository.FindUserByLogin(userLogin) == null)
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand",
            //                CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            //        }

            //        userLogin = ChangedStatusBlockingUser(userLogin, command, true, true);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {userLogin} заблокирован."));
            return clients.Caller.SendAsync("ReceiveCommand", "");
        }

        /// <summary>
        /// Метод разблокироует пользователя в приложении
        /// </summary>
        public Task UserPardon(UserDTO user, string command, IHubCallerClients clients)
        {
            //        if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }))
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            //        }

            //        userLogin = Regex.Replace(command, @"^//user\spardon\s", String.Empty);
            //        if (userRepository.FindUserByLogin(userLogin) == null)
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand",
            //                CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            //        }
            //        userLogin = ChangedStatusBlockingUser(userLogin, command, false);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {userLogin} разблокирован."));
            return clients.Caller.SendAsync("ReceiveCommand", "");
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        public Task TemporaryUserBlock(UserDTO user, string command, IHubCallerClients clients)
        {
            //        if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }))
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            //        }

            //        userLogin = Regex.Match(command, @"//user\sban\s(\w+)\s-m\s\d*$").Groups[1].Value;
            //        if (userRepository.FindUserByLogin(userLogin) == null)
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand",
            //                CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            //        }

            //        userLogin = ChangedStatusBlockingUser(userLogin, command, true);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {userLogin} заблокирован."));
            return clients.Caller.SendAsync("ReceiveCommand", "");
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        public Task SetModerationRole(UserDTO user, string command, IHubCallerClients clients)
        {
            //        if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator" }))
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            //        }

            //        userLogin = Regex.Match(command, @"//user\smoderator\s(\w+)\s-n$").Groups[1].Value;

            //        if (userRepository.FindUserByLogin(userLogin) == null)
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand",
            //                CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            //        }

            //        user = userRepository.FindUserByLogin(userLogin);
            //        user.RoleId = ROLE_MODERATOR;
            //        userRepository.EditUser(user);

            //        return clients.Caller.SendAsync("ReceiveCommand",
            //            CommandHandler.CreateCommandInfo($"Пользователь {user.Login} был назначен модератором."));
            return clients.Caller.SendAsync("ReceiveCommand", "");
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        public Task RemoveModerationRole(UserDTO user, string command, IHubCallerClients clients)
        {
            //        if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator" }))
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            //        }

            //        userLogin = Regex.Match(command, @"//user\smoderator\s(\w+)\s-d$").Groups[1].Value;
            //        if (userRepository.FindUserByLogin(userLogin) == null)
            //        {
            //            return clients.Caller.SendAsync("ReceiveCommand",
            //                CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            //        }

            //        user = userRepository.FindUserByLogin(userLogin);
            //        user.RoleId = ROLE_REGULAR_USER;
            //        userRepository.EditUser(user);
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
        public string ChangedStatusBlockingUser(string userName, string command, bool blockStatus, bool isIndefiniteBlock = false)
        {
            //        User user = userRepository.FindUserByLogin(userLogin);
            //        user.Loked = blockStatus;

            //        if (isIndefiniteBlock)
            //        {
            //            user.DateUnblock = DateTime.Now.AddYears(INDEFINITE_BLOCKING);
            //        }
            //        else
            //        {
            //            user.DateUnblock = TimeComputer.CalculateUnlockDate(command, @"//user\sban\s\w+\s-m\s(\d*)$");
            //        }

            //        userRepository.EditUser(user);

            //        return user.Login;
            //    }
            return "yes";
        }
    }
}
