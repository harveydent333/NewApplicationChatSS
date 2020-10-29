using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Infrastructure.Validators
{
    public class RoomValidator : IRoomValidator
    {
        public IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;

        public RoomValidator(IUnitOfWork uow, UserManager<User> userManager)
        {
            _userManager = userManager;
            Database = uow;
        }

        /// <summary>
        /// Метод проверяет есть ли в базе данных запись с таким же именем комнаты
        /// </summary>
        /// <returns>Возвращает True если записей нет и False если уже присутсвуют</returns>
        public bool UniquenessCheckRoom(string roomName)
        {
            return Database.Rooms.FindByName(roomName) == null;
        }

        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        public async Task<bool> CommandAccessCheckAsync(User user, IEnumerable<string> allowedRoles, string nameProcessedRoom)
        {
            if (IsOwnerRoom(user.Id, nameProcessedRoom))
            {
                return true;
            }

            foreach (string role in allowedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, role))
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
