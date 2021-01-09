using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Repositories.Models;
using NewAppChatSS.Hubs.Interfaces.ValidatorInterfaces;

namespace NewAppChatSS.Hubs.Infrastructure.Validators
{
    public class UserValidator : IUserValidator
    {
        private readonly IMemberRepository memberRepository;
        private readonly IKickedOutRepository kickedOutRepository;
        private readonly IMutedUserRepository mutedUserRepository;
        private readonly UserManager<User> userManager;

        public UserValidator(
            IMemberRepository memberRepository,
            IMutedUserRepository mutedUserRepository,
            IKickedOutRepository kickedOutRepository,
            UserManager<User> userManager)
        {
            this.memberRepository = memberRepository;
            this.mutedUserRepository = mutedUserRepository;
            this.kickedOutRepository = kickedOutRepository;
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
            var mutedUser = await mutedUserRepository.GetFirstOrDefaultAsync(new MutedUserModel { UserId = userId, RoomId = roomId });

            if (mutedUser != null)
            {
                if (mutedUser.DateUnmute > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await mutedUserRepository.DeleteAsync(mutedUser);
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

            var mutedUser = await mutedUserRepository.GetFirstOrDefaultAsync(new MutedUserModel { UserId = userId, RoomId = roomId });

            if (mutedUser != null)
            {
                if (mutedUser.DateUnmute > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await mutedUserRepository.DeleteAsync(mutedUser);
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
            var kicked = await kickedOutRepository.GetFirstOrDefaultAsync(new KickedOutModel { UserId = userId, RoomId = roomId });

            if (kicked != null)
            {
                if (kicked.DateUnkick > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await kickedOutRepository.DeleteAsync(kicked);
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
            var user = await userManager.FindByNameAsync(userName);

            var kicked = await kickedOutRepository.GetFirstOrDefaultAsync(new KickedOutModel { UserId = user.Id, RoomId = roomId });

            if (kicked != null)
            {
                if (kicked.DateUnkick > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    await kickedOutRepository.DeleteAsync(kicked);
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
        public async Task<bool> IsUserInGroupById(string userId, string roomId)
        {
            var members = await memberRepository.GetAsync(new MemberModel { UserId = userId });
            var memberIds = members.Select(m => m.RoomId).ToList();

            return memberIds.Contains(roomId);
        }

        /// <summary>
        /// Метод проверяет, является ли пользователь участником комнаты
        /// </summary>
        public async Task<bool> IsUserInGroupByNameAsync(string userName, string roomId)
        {
            var user = await userManager.FindByNameAsync(userName);
            var roomIds = (await memberRepository.GetAsync(new MemberModel { UserId = user.Id })).Select(m => m.RoomId);
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