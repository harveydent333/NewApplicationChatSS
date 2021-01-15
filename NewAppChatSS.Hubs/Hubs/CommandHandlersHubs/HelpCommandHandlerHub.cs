using System;
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
        /// Метод собирает список доступных команд пользователю
        /// </summary>
        public async Task GetAllowedCommandsAsync(User user, Room room, IHubCallerClients clients)
        {
            allowedCommands.Clear();

            GetCommonCommands(allowedCommands);

            if ((await userManager.IsInRoleAsync(user, RoleConstants.ModeratorRole)) || (await userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole)))
            {
                GetModerCommands(allowedCommands);
            }

            if (await userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole))
            {
                GetAdminCommands(allowedCommands);
            }

            if ((await userManager.IsInRoleAsync(user, RoleConstants.RegularUserRole)) && (user.Id == room.OwnerId))
            {
                GetCommandsForOwnerRoom(allowedCommands);
            }

            allowedCommands = allowedCommands.OrderByDescending(l => l).ToList();

            if (room.TypeId == GlobalConstants.BotRoomType)
            {
                GetCommandsForBotRoom(allowedCommands);
            }

            await clients.Caller.SendAsync("PrintAllowedCommands", CommandHandler.CreateCommandInfo(allowedCommands));
        }

        /// <summary>
        /// Выдать общие команды
        /// Метод добавляет в список доступных команд, команды которые может использовать каждый пользователь в комнате
        /// </summary>
        private void GetCommonCommands(List<string> allowedCommands)
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
        private void GetModerCommands(List<string> allowedCommands)
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
        private void GetAdminCommands(List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.ChangeModeratorRole);
            allowedCommands.Add(ChatCommands.RoomRemove);
            allowedCommands.Add(ChatCommands.RoomRename);
        }

        /// <summary>
        /// Выдать команды доступные в чат-бот комнате.
        /// Метод добавляет в список доступных команд, команды которые можно использовать только в чат-бот комнате
        /// </summary>
        private void GetCommandsForBotRoom(List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.FindVideoOnYouTubeChannel);
            allowedCommands.Add(ChatCommands.GetInfoAboutYouTubeChannel);
            allowedCommands.Add(ChatCommands.GetVideoComments);
        }

        /// <summary>
        /// Выдать команды владельцу комнаты.
        /// Метод добавляет в список доступных команд, команды доступные только владельцу комнаты
        /// </summary>
        private void GetCommandsForOwnerRoom(List<string> allowedCommands)
        {
            allowedCommands.Add(ChatCommands.UserKick);
            allowedCommands.Add(ChatCommands.UserUnmute);
            allowedCommands.Add(ChatCommands.UserMute);
            allowedCommands.Add(ChatCommands.RoomRemove);
            allowedCommands.Add(ChatCommands.RoomRename);
        }
    }
}
