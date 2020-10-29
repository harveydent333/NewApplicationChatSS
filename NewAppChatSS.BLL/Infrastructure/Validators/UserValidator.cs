using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.BLL.DTO;
using NewAppChatSS.BLL.Interfaces.ValidatorInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.BLL.Infrastructure.Validators
{
    public class UserValidator : IUserValidator
    {
        public IUnitOfWork Database { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserValidator(IUnitOfWork uow, UserManager<User> userManager, IMapper mapper)
        {
            Database = uow;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь в приложении
        /// </summary>
        public bool IsUserBlocked(User user)
        {
            if (user.IsLocked && (DateTime.Now > user.DateUnblock))
            {
                user.IsLocked = false;
                _userManager.UpdateAsync(user);
            }

            return user.IsLocked;
        }

        /// <summary>
        /// Метод проверяет, имеет ли пользователь возможность отправлять сообщения в чат в комнате
        /// </summary>
        public bool IsUserMuted(string userId, string roomId)
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
                    Database.MutedUsers.DeleteMutedUser(userId, roomId);
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
        public bool IsUserKicked(string userId, string roomId)
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
                    Database.KickedOuts.DeleteKickedUser(userId, roomId);
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
        public bool IsUserInGroup(string userId, string roomId)
        {
            return Database.Members.GetRooms(userId).Contains(roomId);
        }


        public async Task<bool> CommandAccessCheckAsync(User user, IEnumerable<string> allowedRoles, bool checkOnOwner = false, string processingUserName = "")
        {
            foreach(string role in allowedRoles)
            {
                if (await _userManager.IsInRoleAsync(user, role))
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