using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IRoomCommandHandler
    {
        Task SearchCommandAsync(User currentUser, Room currentRoom, string command, IHubCallerClients calledClients);
    }
}