using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IBotCommandHandlerHub
    {
        Task SearchCommand(string command, IHubCallerClients calledClients);
    }
}
