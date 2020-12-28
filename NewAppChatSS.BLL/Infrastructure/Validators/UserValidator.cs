using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Infrastructure.Validators
{
    public class UserValidator : IUserValidator
    {
        public IUnitOfWork Database { get; set; }

        private readonly UserManager<User> userManager;

        public UserValidator(IUnitOfWork uow, UserManager<User> userManager)
        {
            Database = uow;
            this.userManager = userManager;
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь в приложении
        /// </summary>
        public async Task<bool> IsUserBlocked(User user)
        {
            if (user.IsLocked && (DateTime.Now > user.DateUnblock))
            {
                user.IsLocked = false;
                await userManager.UpdateAsync(user);
            }

            return user.IsLocked;
        }

        /// <summary>
        /// Метод проверяет, имеет ли пользователь возможность отправлять сообщения в чат в комнате
        /// </summary>
        public async Task<bool> IsUserMutedById(string userId, string roomId)
        {
            List<MutedUser> listRoomWhereMutedUser = Database.MutedUsers.GetListMutedRoomForUser(userId).ToList();

            MutedUser mutedUser = listRoomWhereMutedUser.FirstOrDefault(m => m.RoomId == roomId);

            if (mutedUser != null)
            {
                if (mutedUser.DateUnmute > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await Database.MutedUsers.DeleteMutedUserAsync(userId, roomId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> IsUserMutedByNameAsync(string userName, string roomId)
        {
            string userId = (await userManager.FindByNameAsync(userName))?.Id;

            List<MutedUser> listRoomWhereMutedUser = Database.MutedUsers.GetListMutedRoomForUser(userId).ToList();

            MutedUser mutedUser = listRoomWhereMutedUser.FirstOrDefault(m => m.RoomId == roomId);

            if (mutedUser != null)
            {
                if (mutedUser.DateUnmute > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await Database.MutedUsers.DeleteMutedUserAsync(userId, roomId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод проверяет выгнан ли пользователь из комнаты
        /// </summary>
        public async Task<bool> IsUserKickedById(string userId, string roomId)
        {
            List<KickedOut> listKickedOut = Database.KickedOuts.GetListKickedRoomForUser(userId).ToList();

            KickedOut kicked = listKickedOut.FirstOrDefault(k => k.RoomId == roomId);

            if (kicked != null)
            {
                if (kicked.DateUnkick > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await Database.KickedOuts.DeleteKickedUserAsync(userId, roomId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод проверяет выгнан ли пользователь из комнаты
        /// </summary>
        public async Task<bool> IsUserKickedByNameAsync(string userName, string roomId)
        {
            User user = await userManager.FindByNameAsync(userName);
            List<KickedOut> listKickedOut = Database.KickedOuts.GetListKickedRoomForUser(user.Id).ToList();

            KickedOut kicked = listKickedOut.FirstOrDefault(k => k.RoomId == roomId);

            if (kicked != null)
            {
                if (kicked.DateUnkick > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await Database.KickedOuts.DeleteKickedUserAsync(user.Id, roomId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Метод проверяет, является ли пользователь участником комнаты
        /// </summary>
        public bool IsUserInGroupById(string userId, string roomId)
        {
            return Database.Members.GetRoomsIds(userId).Contains(roomId);
        }

        /// <summary>
        /// Метод проверяет, является ли пользователь участником комнаты
        /// </summary>
        public async Task<bool> IsUserInGroupByNameAsync(string userName, string roomId)
        {
            var user = await userManager.FindByNameAsync(userName);
            var roomIds = Database.Members.GetRoomsIds(user.Id);
            return roomIds.Contains(roomId);
        }

        public async Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, bool checkOnOwner = false, string processingUserName = "")
        {
            foreach (string role in allowedRoles)
            {
                if (await userManager.IsInRoleAsync(user, role))
                {
                    return true;
                }
            }

            if (checkOnOwner)
            {
                return user.UserName == processingUserName;
            }

            return false;
        }
    }
}