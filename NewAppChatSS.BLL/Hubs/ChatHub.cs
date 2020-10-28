using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.HubInterfaces;
using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUserCommandHandler _userCommandHandler;

        public ChatHub(UserManager<User> userManager, IMapper mapper, IUserCommandHandler userCommandHandler)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userCommandHandler = userCommandHandler;
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
            UserDTO userDTO = _mapper.Map<UserDTO>(user);

            await _userCommandHandler.SearchCommand(userDTO, comamand, Clients);
        }
    }
}
