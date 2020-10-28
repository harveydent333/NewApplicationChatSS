using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IUserCommandHandler
    {
        Task SearchCommand(UserDTO user, string command, IHubCallerClients calledClients);
        Task RemoveModerationRole(UserDTO user, string command, IHubCallerClients clients);
        Task UserBan(UserDTO user, string command, IHubCallerClients clients);
        Task UserPardon(UserDTO user, string command, IHubCallerClients clients);
        Task TemporaryUserBlock(UserDTO user, string command, IHubCallerClients clients);
        Task SetModerationRole(UserDTO user, string command, IHubCallerClients clients);
        Dictionary<string, string> ParsingUserNames(string stringLogins);
        string ChangedStatusBlockingUser(string userName, string command, bool blockStatus, bool isIndefiniteBlock = false);
    }
}
