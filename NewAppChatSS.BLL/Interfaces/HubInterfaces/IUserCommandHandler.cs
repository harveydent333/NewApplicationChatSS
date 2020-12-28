using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IUserCommandHandler
    {
        Task SearchCommandAsync(User user, string command, IHubCallerClients calledClients);
    }
}
