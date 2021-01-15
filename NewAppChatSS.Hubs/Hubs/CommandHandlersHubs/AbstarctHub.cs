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
        public abstract Dictionary<Regex, Func<string, Task>> Commands { get; }

        protected IHubCallerClients clients;
        protected Room room;
        protected User user;

        /// <summary>
        /// Метод проверяет какое регулярное выражение соответствует полученной команде
        /// по результатам перенаправляет на нужный метод обработки команды
        /// </summary>
        public async Task SearchCommandAsync(User user, Room room, string command, IHubCallerClients clients)
        {
            this.clients = clients;
            this.room = room;
            this.user = user;

            foreach (Regex keyCommand in Commands.Keys)
            {
                if (keyCommand.IsMatch(command))
                {
                    await Commands[keyCommand](command);

                    return;
                }
            }

            await clients.Caller.SendAsync("ReceiveCommand", CommandHandler.CreateCommandInfo(InformationMessages.IncorrectCommand));
        }
    }
}
