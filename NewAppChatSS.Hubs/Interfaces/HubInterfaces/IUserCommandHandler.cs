using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.HubInterfaces
{
    public interface IUserCommandHandler
    {
        // TODO: переписать на все public методы и добавить сюда
        Task SearchCommandAsync(User user, string command);
    }
}
