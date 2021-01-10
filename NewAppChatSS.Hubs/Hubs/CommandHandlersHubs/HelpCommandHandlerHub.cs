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
        private IHubCallerClients clients;

        private List<string> allowedCommands = new List<string>();

        public HelpCommandHandlerHub(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Метод проверяет корректность команды и перенаправляет на метод сбора доступных команд
        /// </summary>
        public Task SearchCommand(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            this.clients = clients;

            if (new Regex(@"^//help$").IsMatch(command))
            {
                return GetAllowedCommands(currentUser, currentRoom);
            }
            else
            {
                return clients.Caller.SendAsync(
                    "ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.IncorrectCommand));
            }
        }

        /// <summary>
        /// Метод собирает список доступных команд пользователю
        /// </summary>
        private async Task<Task> GetAllowedCommands(User currentUser, Room currentRoom)
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
            allowedCommands.Add(ChatCommands.RoomCreate);
            allowedCommands.Add(ChatCommands.RoomRename);
            allowedCommands.Add(ChatCommands.RoomRemove);
            allowedCommands.Add(ChatCommands.DisconnectFromCurrentRoom);
            allowedCommands.Add(ChatCommands.DisconeectFromRoom);
            allowedCommands.Add(ChatCommands.ConnectToRoom);
        }

        /// <summary>
        /// Выдать команды доступные модератору.
        /// Метод добавляет в список доступных команд, команды пользователю с ролью "Модератор"
        /// </summary>
        private void GetModerCommands(ref List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.UserBan);
            allowedCommands.Add(ChatCommands.UserUnban);
            allowedCommands.Add(ChatCommands.UserMute);
            allowedCommands.Add(ChatCommands.UserUnmute);
            allowedCommands.Add(ChatCommands.UserKick);
        }

        /// <summary>
        /// Выдать команды доступные администратору.
        /// Метод добавляет в список доступных команд, команды пользователю с ролью "Администратор"
        /// </summary>
        private void GetAdminCommands(ref List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.ChangeModeratorRole);
            allowedCommands.Add(ChatCommands.RoomRemove);
            allowedCommands.Add(ChatCommands.RoomRename);
        }

        /// <summary>
        /// Выдать команды доступные в чат-бот комнате.
        /// Метод добавляет в список доступных команд, команды которые можно использовать только в чат-бот комнате
        /// </summary>
        private void GetCommandsForBotRoom(ref List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.FindVideoOnYouTubeChannel);
            allowedCommands.Add(ChatCommands.GetInfoAboutYouTubeChannel);
            allowedCommands.Add(ChatCommands.GetVideoComments);
        }

        /// <summary>
        /// Выдать команды владельцу комнаты.
        /// Метод добавляет в список доступных команд, команды доступные только владельцу комнаты
        /// </summary>
        private void GetCommandsForOwnerRoom(ref List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.UserKick);
            allowedCommands.Add(ChatCommands.UserUnmute);
            allowedCommands.Add(ChatCommands.UserMute);
            allowedCommands.Add(ChatCommands.RoomRemove);
            allowedCommands.Add(ChatCommands.RoomRename);
        }
    }
}
