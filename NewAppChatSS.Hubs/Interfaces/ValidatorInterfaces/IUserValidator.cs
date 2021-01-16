using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces
{
    public interface IUserValidator
    {
        /// <summary>
        /// Метод указывает, действительно ли указанный объект <see cref="User"/> является null
        /// </summary>
        /// <param name="user">Объект <see cref="User"/></param>
        /// <returns>Значение true если объект равен null, в противном случае false </returns>
        Task<bool> IsNullUserAsync(User user);

        Task<bool> IsUserBlockedAsync(User user);

        Task<bool> IsUserMutedAsync(string userId, string roomId);

        Task<bool> IsUserKickedAsync(string userId, string roomId);

        Task<bool> IsUserInGroupAsync(string userId, string roomId);

        Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, bool checkOnOwner, string processingUserName);
    }
}
