using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.Hubs.Interfaces.HubInterfaces;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    /// <summary>
    /// Обработчик команды help
    /// </summary>
    public sealed class HelpCommandHandlerHub : IHelpCommandHandlerHub
    {
        private readonly UserManager<User> userManager;

        private List<string> allowedCommands = new List<string>();

        public HelpCommandHandlerHub(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Метод проверяет корректность команды и перенаправляет на метод сбора доступных команд
        /// </summary>
        public Task SearchCommand(User currentUser, Room currentRoom, string command, IHubCallerClients calledClients)
        {
            if (new Regex(@"^//help$").IsMatch(command))
            {
                return GetAllowedCommands(currentUser, currentRoom, calledClients);
            }
            else
            {
                return calledClients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo("Неверная команда"));
            }
        }

        /// <summary>
        /// Метод собирает список доступных команд пользователю
        /// </summary>
        private async Task<Task> GetAllowedCommands(User currentUser, Room currentRoom, IHubCallerClients clients)
        {
            allowedCommands.Clear();

            GetCommonCommands(ref allowedCommands);

            if ((await userManager.IsInRoleAsync(currentUser, "Moderator")) || (await userManager.IsInRoleAsync(currentUser, "Administrator")))
            {
                GetModerCommands(ref allowedCommands);
            }

            if (await userManager.IsInRoleAsync(currentUser, "Administrator"))
            {
                GetAdminCommands(ref allowedCommands);
            }

            if ((await userManager.IsInRoleAsync(currentUser, "RegularUser")) && (currentUser.Id == currentRoom.OwnerId))
            {
                GetCommandsForOwnerRoom(ref allowedCommands);
            }

            allowedCommands = allowedCommands.OrderByDescending(l => l).ToList();

            if (currentRoom.TypeId == GlobalConstants.BotRoomType)
            {
                GetCommandsForBotRoom(ref allowedCommands);
            }

            return clients.Caller.SendAsync("PrintAllowedCommands", CommandHandler.CreateCommandInfo(allowedCommands));
        }

        /// <summary>
        /// Выдать общие команды
        /// Метод добавляет в список доступных команд, команды которые может использовать каждый пользователь в комнате
        /// </summary>
        private void GetCommonCommands(ref List<string> allowedCommands)
        {
            allowedCommands.Add("//user rename {login пользователя}||{Новый login пользователя}");
            allowedCommands.Add("//user rename {login пользователя}||{Новый login пользователя}");
            allowedCommands.Add("//room create {Название комнаты} [-c] or [-b]");
            allowedCommands.Add("//room remove {Название комнаты}");
            allowedCommands.Add("//room disconnect");
            allowedCommands.Add("//room disconnect {Название комнаты}");
            allowedCommands.Add("//room connect {Название комнаты} -l {login пользователя}");
        }

        /// <summary>
        /// Выдать команды доступные модератору.
        /// Метод добавляет в список доступных команд, команды пользователю с ролью "Модератор"
        /// </summary>
        private void GetModerCommands(ref List<string> allowedCommands)
        {
            allowedCommands.Add("//user ban {login пользователя} [-m {Количество минут}]");
            allowedCommands.Add("//user pardon {login пользователя}");
            allowedCommands.Add("//room mute -l {login пользователя} [-m {Количество минут}]");
            allowedCommands.Add("//room speak -l {login пользователя}");
            allowedCommands.Add("//room disconnect {Название комнаты} [-l {login пользователя}] [-m {Количество минут}]");
        }

        /// <summary>
        /// Выдать команды доступные администратору.
        /// Метод добавляет в список доступных команд, команды пользователю с ролью "Администратор"
        /// </summary>
        private void GetAdminCommands(ref List<string> allowedCommands)
        {
            allowedCommands.Add("//user moderator {login пользователя} [-n] or [-d]");
            allowedCommands.Add("//room remove {Название комнаты}");
            allowedCommands.Add("//room rename {Название комнаты}");
        }

        /// <summary>
        /// Выдать команды доступные в чат-бот комнате.
        /// Метод добавляет в список доступных команд, команды которые можно использовать только в чат-бот комнате
        /// </summary>
        private void GetCommandsForBotRoom(ref List<string> allowedCommands)
        {
            allowedCommands.Add("//find {название канала}||{название видео} [-v] [-l]");
            allowedCommands.Add("//info {название канала}");
            allowedCommands.Add("//videoCommentRandom {название канала}||{Название ролика}");
        }

        /// <summary>
        /// Выдать команды владельцу комнаты.
        /// Метод добавляет в список доступных команд, команды доступные только владельцу комнаты
        /// </summary>
        private void GetCommandsForOwnerRoom(ref List<string> allowedCommands)
        {
            allowedCommands.Add("//room disconnect {Название комнаты} [-l {login пользователя}] [-m {Количество минут}]");
            allowedCommands.Add("//room speak -l {login пользователя}");
            allowedCommands.Add("//room mute -l {login пользователя} [-m {Количество минут}]");
            allowedCommands.Add("//room remove {Название комнаты}");
            allowedCommands.Add("//room rename {Название комнаты}");
        }
    }
}
