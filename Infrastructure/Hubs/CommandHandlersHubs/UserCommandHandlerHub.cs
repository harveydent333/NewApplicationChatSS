using Data.Models.Users;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команд взаимодействия с пользователем
    /// </summary>
    public class UserCommandHandlerHub : Hub
    {
        const Int32 ROLE_REGULAR_USER = 1;
        const Int32 ROLE_MODERATOR = 2;
        const Int32 INDEFINITE_BLOCKING = 100;

        private static String userLogin;

        private static IUserRepository userRepository;

        public UserCommandHandlerHub(IUserRepository userRep)
        {
            userRepository = userRep;
        }

        private static Dictionary<Regex, Func<User, String, IHubCallerClients, Task>> userCommands = new Dictionary<Regex, Func<User, String, IHubCallerClients, Task>>
        {
            [new Regex(@"^//user\srename\s\w+\W\W\w+\S$")] = UserRename,
            [new Regex(@"^//user\sban\s\w+\S$")] = UserBan,
            [new Regex(@"^//user\sban\s\w+\s-m\s\d*$")] = TemporaryUserBlock,
            [new Regex(@"^//user\spardon\s\w+\S$")] = UserPardon,
            [new Regex(@"^//user\smoderator\s\w+\s-n$")] = SetModerationRole,
            [new Regex(@"^//user\smoderator\s\w+\s-d$")] = RemoveModerationRole,
        };

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public Task SearchCommand(User user, String command, IHubCallerClients calledClients)
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
        public static Task UserRename(User user, String command, IHubCallerClients clients)
        {
            String ownerLogin = user.Login;
            Dictionary<String, String> userLogins = ParsingLogins(command);

            if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }, true, userLogins["oldLogin"]))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            if (userRepository.FindUserByLogin(userLogins["oldLogin"]) == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand",
                    CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogins["oldLogin"]} не найден."));
            }

            user = userRepository.FindUserByLogin(userLogins["oldLogin"]);
            if (UserValidator.UniquenessCheckUser(userLogins["newLogin"]))
            {
                user.Login = userLogins["newLogin"];
                userRepository.EditUser(user);

                if (UserValidator.IsOwnerLogin(ownerLogin, userLogins["oldLogin"]))
                {
                    return clients.Caller.SendAsync("UserRenameClient", userLogins["newLogin"],
                        CommandHandler.CreateCommandInfo($"Логин был успешно изменен на {userLogins["newLogin"]}"));
                }
                else
                    return clients.Caller.SendAsync("ReceiveCommand", 
                        CommandHandler.CreateCommandInfo($"Логин пользователя {userLogins["oldLogin"]} был успешно изменен " +
                            $"на {userLogins["newLogin"]}"));
            }

            return clients.Caller.SendAsync("ReceiveCommand", 
                CommandHandler.CreateCommandInfo($"Логин {userLogins["oldLogin"]} уже занят"));
        }

        /// <summary>
        /// Метод бессрочно блокирует пользоваетля в приложении
        /// </summary>
        public static Task UserBan(User user, String command, IHubCallerClients clients)
        {
            if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }
            userLogin = Regex.Replace(command, @"^//user\sban\s", String.Empty);

            if (userRepository.FindUserByLogin(userLogin) == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand", 
                    CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            }

            userLogin = ChangedStatusBlockingUser(userLogin, command, true, true);

            return clients.Caller.SendAsync("ReceiveCommand", 
                CommandHandler.CreateCommandInfo($"Пользователь {userLogin} заблокирован."));
        }

        /// <summary>
        /// Метод разблокироует пользователя в приложении
        /// </summary>
        public static Task UserPardon(User user, String command, IHubCallerClients clients)
        {
            if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            userLogin = Regex.Replace(command, @"^//user\spardon\s", String.Empty);
            if (userRepository.FindUserByLogin(userLogin) == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand", 
                    CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            }
            userLogin = ChangedStatusBlockingUser(userLogin, command, false);

            return clients.Caller.SendAsync("ReceiveCommand", 
                CommandHandler.CreateCommandInfo($"Пользователь {userLogin} разблокирован."));
        }

        /// <summary>
        /// Метод временно блокирует пользоваетля
        /// </summary>
        public static Task TemporaryUserBlock(User user, String command, IHubCallerClients clients)
        {
            if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator", "Moderator" }))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            userLogin = Regex.Match(command, @"//user\sban\s(\w+)\s-m\s\d*$").Groups[1].Value;
            if (userRepository.FindUserByLogin(userLogin) == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand", 
                    CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            }

            userLogin = ChangedStatusBlockingUser(userLogin, command, true);

            return clients.Caller.SendAsync("ReceiveCommand", 
                CommandHandler.CreateCommandInfo($"Пользователь {userLogin} заблокирован."));
        }

        /// <summary>
        /// Метод назначает роль "модератор" пользователю
        /// </summary>
        public static Task SetModerationRole(User user, String command, IHubCallerClients clients)
        {
            if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator" }))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            userLogin = Regex.Match(command, @"//user\smoderator\s(\w+)\s-n$").Groups[1].Value;

            if (userRepository.FindUserByLogin(userLogin) == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand", 
                    CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            }

            user = userRepository.FindUserByLogin(userLogin);       
            user.RoleId = ROLE_MODERATOR;
            userRepository.EditUser(user);                           

            return clients.Caller.SendAsync("ReceiveCommand", 
                CommandHandler.CreateCommandInfo($"Пользователь {user.Login} был назначен модератором."));
        }

        /// <summary>
        /// Метод назначает роль "обычный пользователь" пользователю
        /// </summary>
        public static Task RemoveModerationRole(User user, String command, IHubCallerClients clients)
        {
            if (!UserValidator.CommandAccessCheck(user, new String[] { "Administrator" }))
            {
                return clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Отказано в доступе."));
            }

            userLogin = Regex.Match(command, @"//user\smoderator\s(\w+)\s-d$").Groups[1].Value;
            if (userRepository.FindUserByLogin(userLogin) == null)
            {
                return clients.Caller.SendAsync("ReceiveCommand", 
                    CommandHandler.CreateCommandInfo($"Пользователь с логином {userLogin} не найден."));
            }

            user = userRepository.FindUserByLogin(userLogin);               
            user.RoleId = ROLE_REGULAR_USER;                                
            userRepository.EditUser(user);                                  
            return clients.Caller.SendAsync("ReceiveCommand", 
                CommandHandler.CreateCommandInfo($"Пользователь {user.Login} был разжалован до обычного пользователя."));
        }

        /// <summary>
        /// Метод получает старый и новый логин из текста переданной команды
        /// </summary>
        public static Dictionary<String, String> ParsingLogins(String StringLogins)
        {
            return new Dictionary<String, String>
            {
                ["oldLogin"] = Regex.Match(StringLogins, @"//user rename (\w+)\W\W(\w+)$").Groups[1].Value,
                ["newLogin"] = Regex.Match(StringLogins, @"//user rename (\w+)\W\W(\w+)$").Groups[2].Value
            };
        }

        /// <summary>
        /// Метод меняет статус роли пользователя
        /// </summary>
        public static String ChangedStatusBlockingUser(String userLogin, String command, Boolean blockStatus, Boolean isIndefiniteBlock = false)
        {
            User user = userRepository.FindUserByLogin(userLogin);
            user.Loked = blockStatus;

            if (isIndefiniteBlock)
            {
                user.DateUnblock = DateTime.Now.AddYears(INDEFINITE_BLOCKING);
            }
            else
            {
                user.DateUnblock = TimeComputer.CalculateUnlockDate(command, @"//user\sban\s\w+\s-m\s(\d*)$");
            }

            userRepository.EditUser(user);

            return user.Login;
        }
    }
}