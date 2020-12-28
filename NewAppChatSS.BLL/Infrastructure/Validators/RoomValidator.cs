using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Infrastructure.Validators
{
    public class RoomValidator : IRoomValidator
    {
        public IUnitOfWork Database { get; set; }

        private readonly UserManager<User> userManager;

        public RoomValidator(IUnitOfWork uow, UserManager<User> userManager)
        {
            this.userManager = userManager;
            Database = uow;
        }

        /// <summary>
        /// Метод проверяет есть ли в базе данных запись с таким же именем комнаты
        /// </summary>
        /// <returns>Возвращает True если записей нет и False если уже присутсвуют</returns>
        public bool UniquenessCheckRoom(string roomName)
        {
            // TODO: Сделать проверку по кол-во, реализовать общий метод и если возвращает не 0, то вернуть true
            var rooms = Database.Rooms.FindByName(roomName);
            return rooms != null;
        }

        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        public async Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, string nameProcessedRoom)
        {
            if (IsOwnerRoom(user.Id, nameProcessedRoom))
            {
                return true;
            }

            foreach (string role in allowedRoles)
            {
                if (await userManager.IsInRoleAsync(user, role))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет является ли пользователь владельцем комнаты
        /// </summary>
        public bool IsOwnerRoom(string userId, string nameProcessedRoom)
        {
            if (Database.Rooms.FindByName(nameProcessedRoom).OwnerId == userId)
            {
                return true;
            }

            return false;
        }
    }
}
