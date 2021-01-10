using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NewAppChatSS.Hubs.Interfaces.HubInterfaces
{
    public interface IBotCommandHandlerHub
    {
        // TODO: переписать на все public методы и добавить сюда
        Task SearchCommand(string command, IHubCallerClients clients);
    }
}
