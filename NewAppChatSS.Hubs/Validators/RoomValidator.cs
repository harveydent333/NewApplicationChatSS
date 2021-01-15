using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;

namespace NewAppChatSS.Hubs.Infrastructure.Validators
{
    public class RoomValidator : IRoomValidator
    {
        private readonly UserManager<User> userManager;
        private readonly IRoomRepository roomRepository;

        public RoomValidator(
            IRoomRepository roomRepository,
            UserManager<User> userManager)
        {
            this.roomRepository = roomRepository;
            this.userManager = userManager;
        }

        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        public async Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, string nameProcessedRoom)
        {
            if (await IsOwnerRoomAsync(user.Id, nameProcessedRoom))
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
        public async Task<bool> IsOwnerRoomAsync(string userId, string nameProcessedRoom)
        {
            var room = await roomRepository.GetFirstOrDefaultAsync(new RoomModel { RoomName = nameProcessedRoom });

            if (room.OwnerId == userId)
            {
                return true;
            }

            return false;
        }
    }
}
