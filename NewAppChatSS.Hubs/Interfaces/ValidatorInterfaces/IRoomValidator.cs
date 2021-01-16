using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces
{
    public interface IRoomValidator
    {
        /// <summary>
        /// Метод указывает, действительно ли указанный объект <see cref="Room"/> является null
        /// </summary>
        /// <param name="room">Объект <see cref="Room"/></param>
        /// <returns>Значение true если объект равен null, в противном случае false</returns>
        Task<bool> InNullRoomAsync(Room room);

        /// <summary>
        /// Метод проверяет, принадлежат ли данные "главной комнате"
        /// </summary>
        /// <param name="roomId">Идентификатор объекта <see cref="Room"/></param>
        /// <returns>Значение true если идентификатор объекта равен идентификатору объекта главной комнаты, иначе false</returns>
        Task<bool> IsMainRoomAsync(string roomId);

        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        Task<bool> CommandAccessCheckAsync(User user, Room room, List<string> allowedRoles);

        /// <summary>
        /// Метод проверяет является ли пользователь владельцем комнаты
        /// </summary>
        Task<bool> IsOwnerRoomAsync(User user, Room room);
    }
}
