using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.Hubs.Hubs.CommandHandlersHubs;

namespace NewAppChatSS.Hubs.Interfaces.HubInterfaces
{
    public interface IRoomCommandHandler : IAbstractHub
    {
        // TODO: переписать на все public методы и добавить сюда
    }
}