using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.HubInterfaces
{
    public interface IRoomCommandHandler
    {
        // TODO: переписать на все public методы и добавить сюда
        Task SearchCommandAsync(User currentUser, Room currentRoom, string command);
    }
}