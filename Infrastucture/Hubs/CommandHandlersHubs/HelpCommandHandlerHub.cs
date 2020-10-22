using AppChatSS.Infrastucture;
using AppChatSS.Models.Rooms;
using AppChatSS.Models.Users;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppChatSS.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команды help
    /// </summary>
    public class HelpCommandHandlerHub
    {
        const Int32 REGULAR_TYPE_ROOM = 1;
        const Int32 PRITVATE_TYPE_ROOM = 2;
        const Int32 BOT_TYPE_ROOM = 3;

        const Int32 ROLE_REGULAR_USER = 1;
        const Int32 ROLE_MODERATOR = 2;
        const Int32 ROLE_ADMINISTRATOR = 3;

        /// <summary>
        /// Метод проверяет корректность команды и перенаправляет на метод сбора доступных команд
        /// </summary>
        public static Task SearchCommand(User currentUser, Room currentRoom, String command, IHubCallerClients calledClients)
        {
            if (new Regex(@"^//help$").IsMatch(command))
            {
                return GetAllowedCommands(currentUser, currentRoom, command, calledClients);
            }
            else
            {
                return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
            }
        }

        /// <summary>
        /// Метод собирает список доступных команд пользователю
        /// </summary>
        public static Task GetAllowedCommands(User currentUser, Room currentRoom, String command, IHubCallerClients clients)
        {
            List<String> allowedCommands = new List<String>();

            GetCommonCommands(ref allowedCommands);

            if ((currentUser.RoleId == ROLE_MODERATOR) || (currentUser.RoleId == ROLE_ADMINISTRATOR))
            {
                GetModerCommands(ref allowedCommands);
            }

            if (currentUser.RoleId == ROLE_ADMINISTRATOR)
            {
                GetAdminCommands(ref allowedCommands);
            }

            if ((currentUser.RoleId == ROLE_REGULAR_USER) && (currentUser.Id == currentRoom.OwnerId))
            {
                GetCommandsForOwnerRoom(ref allowedCommands);
            }

            allowedCommands = allowedCommands.OrderByDescending(l => l).ToList();

            if (currentRoom.TypeId == BOT_TYPE_ROOM)
            {
                GetCommandsForBotRoom(ref allowedCommands);
            }

            return clients.Caller.SendAsync("PrintAllowedCommands", CommandHandler.CreateCommandInfo(allowedCommands));
        }

        /// <summary>
        /// Выдать общие команды
        /// Метод добавляет в список доступных команд, команды которые может использовать каждый пользователь в комнате
        /// </summary>
        public static void GetCommonCommands(ref List<String> allowedCommands)
        {
            allowedCommands.AddRange(new String[]
            {
                "//user rename {login пользователя}||{Новый login пользователя}",
                "//room create {Название комнаты} [-c] or [-b]",
                "//room remove {Название комнаты}",
                "//room disconnect",
                "//room disconnect {Название комнаты}",
                "//room connect {Название комнаты} -l {login пользователя}",
            });
        }

        /// <summary>
        /// Выдать команды доступные модератору.
        /// Метод добавляет в список доступных команд, команды пользователю с ролью "Модератор"
        /// </summary>
        public static void GetModerCommands(ref List<String> allowedCommands)
        {
            allowedCommands.AddRange(new String[]
            {
                "//user ban {login пользователя} [-m {Количество минут}]",
                "//user pardon {login пользователя}",
                "//room mute -l {login пользователя} [-m {Количество минут}]",
                "//room speak -l {login пользователя}",
                "//room disconnect {Название комнаты} [-l {login пользователя}] [-m {Количество минут}]",
            });
        }

        /// <summary>
        /// Выдать команды доступные администратору.
        /// Метод добавляет в список доступных команд, команды пользователю с ролью "Администратор"
        /// </summary>
        public static void GetAdminCommands(ref List<String> allowedCommands)
        {
            allowedCommands.AddRange(new String[]
            {
                "//user moderator {login пользователя} [-n] or [-d]",
                "//room remove {Название комнаты}",
                "//room rename {Название комнаты}",
            });
        }

        /// <summary>
        /// Выдать команды доступные в чат-бот комнате.
        /// Метод добавляет в список доступных команд, команды которые можно использовать только в чат-бот комнате
        /// </summary>
        public static void GetCommandsForBotRoom(ref List<String> allowedCommands)
        {
            allowedCommands.InsertRange(0, new String[]
            {
                "//find {название канала}||{название видео} [-v] [-l]",
                "//info {название канала}",
                "//videoCommentRandom {название канала}||{Название ролика}"
            });
        }

        /// <summary>
        /// Выдать команды владельцу комнаты.
        /// Метод добавляет в список доступных команд, команды доступные только владельцу комнаты
        /// </summary>
        public static void GetCommandsForOwnerRoom(ref List<String> allowedCommands)
        {
            allowedCommands.AddRange(new String[]
            {
                "//room disconnect {Название комнаты} [-l {login пользователя}] [-m {Количество минут}]",
                "//room speak -l {login пользователя}",
                "//room mute -l {login пользователя} [-m {Количество минут}]",
                "//room remove {Название комнаты}",
                "//room rename {Название комнаты}",
            });
        }
    }
}
