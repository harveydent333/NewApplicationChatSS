using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IHelpCommandHandlerHub
    {
        Task SearchCommand(User currentUser, Room currentRoom, string command, IHubCallerClients calledClients);
    }
}
