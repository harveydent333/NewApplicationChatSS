using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.HubInterfaces
{
    public interface IHelpCommandHandlerHub
    {
        Task GetAllowedCommandsAsync(User currentUser, Room currentRoom, IHubCallerClients clients);
        // TODO: переписать на все public методы и добавить сюда
    }
}
