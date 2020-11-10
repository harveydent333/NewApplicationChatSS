using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IUserCommandHandler
    {
        Task SearchCommandAsync(User user, string command, IHubCallerClients calledClients);
    }
}
