using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces
{
    public interface IRoomValidator
    {
        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, string nameProcessedRoom);

        /// <summary>
        /// Метод проверяет является ли пользователь владельцем комнаты
        /// </summary>
        Task<bool> IsOwnerRoom(string userId, string nameProcessedRoom);
    }
}
