using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IRoomCommandHandler
    {
        Task SearchCommandAsync(User currentUser, Room currentRoom, string command, IHubCallerClients calledClients);        
        
        Task CreateRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task CreatePrivateRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task CreateBotRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> RemoveRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> RenameRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> ConnectionToRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task DisconnectFromCurrenctRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task DisconnectFromRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> KickUserOutRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> TemporaryKickUserOutRoom(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> MuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> TemporaryMuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        Task<Task> UnmuteUser(User currentUser, Room currentRoom, string command, IHubCallerClients clients);

        string CheckDataForKickedUser(string nameProcessedRoom, string loginProcessedUser, string idProcessedRoom, string idProcessedUser);
    }
}