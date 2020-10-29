using Microsoft.AspNetCore.SignalR;
using NewAppChatSS.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Interfaces.HubInterfaces
{
    public interface IUserCommandHandler
    {
        Task SearchCommandAsync(User user, string command, IHubCallerClients calledClients);

        Task<Task> RemoveModerationRoleAsync(User user, string command, IHubCallerClients clients);

        Task<Task> UserRenameAsync(User user, string command, IHubCallerClients clients);

        Task<Task> UserBanAsync(User user, string command, IHubCallerClients clients);

        Task<Task> UserPardonAsync(User user, string command, IHubCallerClients clients);

        Task<Task> TemporaryUserBlockAsync(User user, string command, IHubCallerClients clients);

        Task<Task> SetModerationRoleAsync(User user, string command, IHubCallerClients clients);

        Dictionary<string, string> ParsingUserNames(string stringLogins);

        Task<string> ChangedStatusBlockingUserAsync(string userName, string command, bool blockStatus, bool isIndefiniteBlock = false);
    }
}
