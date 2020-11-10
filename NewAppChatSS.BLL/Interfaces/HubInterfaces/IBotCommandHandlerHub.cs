using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IBotCommandHandlerHub
    {
        Task SearchCommand(string command, IHubCallerClients calledClients);
    }
}
