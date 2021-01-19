using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Hubs.CommandHandlersHubs
{
    public abstract class AbstarctHub : IAbstractHub
    {
        public virtual Dictionary<Regex, Func<string, Task>> Commands { get; }

        protected IHubCallerClients clients;
        protected Room currentRoom;
        protected User currentUser;

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public async Task SearchCommandAsync(User currentUser, Room currentRoom, string command, IHubCallerClients clients)
        {
            this.clients = clients;
            this.currentRoom = currentRoom;

            if (currentRoom == null)
            {
                throw new Exception(ValidationMessageConstants.CurrentRoomNotFound);
            }

            this.currentUser = currentUser;

            if (currentUser == null)
            {
                throw new Exception(ValidationMessageConstants.CurrentUserNotFound);
            }

            foreach (Regex keyCommand in Commands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    await Commands[keyCommand](command);

                    return;
                }
            }

            await SendResponseMessage(ValidationMessageConstants.IncorrectCommand);
        }

        protected async Task SendResponseMessage(string informationMessage)
        {
            var responseMessage = CommandHandler.CreateResponseMessage(informationMessage);
            await clients.Caller.SendAsync("ReceiveCommand", responseMessage);
        }
    }
}
