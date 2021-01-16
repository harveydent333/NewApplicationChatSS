using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;

namespace NewAppChatSS.Hubs.Infrastructure.Validators
{
    public class RoomValidator : IRoomValidator
    {
        private readonly UserManager<User> userManager;

        public RoomValidator(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Метод указывает, действительно ли указанный объект <see cref="Room"/> является null
        /// </summary>
        /// <param name="room">Объект <see cref="Room"/></param>
        /// <returns>Значение true если объект равен null, в противном случае false</returns>
        public async Task<bool> InNullRoomAsync(Room room)
        {
            if (room == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет, принадлежат ли данные "главной комнате"
        /// </summary>
        /// <param name="roomId">Идентификатор объекта <see cref="Room"/></param>
        /// <returns>Значение true если идентификатор объекта равен идентификатору объекта главной комнаты, иначе false</returns>
        public async Task<bool> IsMainRoomAsync(string roomId)
        {
            if (roomId == GlobalConstants.MainRoomId)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        public async Task<bool> CommandAccessCheckAsync(User user, Room room, List<string> allowedRoles)
        {
            var userRoles = await userManager.GetRolesAsync(user);

            if (await IsOwnerRoomAsync(user, room))
            {
                return true;
            }

            foreach (string role in allowedRoles)
            {
                if (userRoles.Contains(role))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет является ли пользователь владельцем комнаты
        /// </summary>
        public async Task<bool> IsOwnerRoomAsync(User user, Room room)
        {
            if (room.OwnerId == user.Id)
            {
                return true;
            }

            return false;
        }
    }
}
