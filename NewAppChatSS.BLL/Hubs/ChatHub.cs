using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserCommandHandler _userCommandHandler;
        private readonly IRoomCommandHandler _roomCommandHandler;
        private IUnitOfWork Database { get; set; }

        public ChatHub(UserManager<User> userManager, IUserCommandHandler userCommandHandler, IRoomCommandHandler roomCommandHandler, IUnitOfWork uow)
        {
            Database = uow;
            _userManager = userManager;
            _userCommandHandler = userCommandHandler;
            _roomCommandHandler = roomCommandHandler;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// Получение команды взаимодействия с пользователем.
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя.
        /// Перенаправляет команду в обработчик команд взаимодействия с пользователями.
        /// </summary>
        public async Task ReceivingUserInteractionCommand(string userEmail, string comamand)
        {
            User user = await _userManager.FindByEmailAsync(userEmail);
            await _userCommandHandler.SearchCommandAsync(user, comamand, Clients);
        }

        /// <summary>
        ///Получение команды взаимодействия с комнатой
        /// Вызываемый метод клиентом, получает объект пользователя по принятому id пользователя, а также комнаты
        /// Перенаправляет команду в обработчик команд взаимодействия с комнатами
        /// </summary>
        public async Task ReceivingRoomInteractionCommand(string userEmail, string roomId, string command)
        {
            User user = await _userManager.FindByEmailAsync(userEmail);
            Room room = Database.Rooms.FindById(roomId);
            await _roomCommandHandler.SearchCommandAsync(user, room, command, Clients);
        }
    }
}
