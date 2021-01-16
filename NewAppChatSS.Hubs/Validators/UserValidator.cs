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
        /// Метод указывает, действительно ли указанный объект <see cref="User"/> является null
        /// </summary>
        /// <param name="user">Объект <see cref="User"/></param>
        /// <returns>Значение true если объект равен null, в противном случае false </returns>
        public async Task<bool> IsNullUserAsync(User user)
        {
            if (user == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь в приложении
        /// </summary>
        public async Task<bool> IsUserBlockedAsync(User user)
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
        /// <param name="userId">Идентификатор объект <see cref="User"/></param>
        /// <param name="roomId">Идентификатор объект <see cref="Room"/></param>
        /// <returns>Значение true если пользователь находится в муте и не может отправлять сообщения в чат,
        ///          в противном случае значние false</returns>
        public async Task<bool> IsUserMutedAsync(string userId, string roomId)
        {
            var mutedUserModel = new MutedUserModel { UserId = userId, RoomId = roomId };
            var mutedUser = await mutedUserRepository.GetFirstOrDefaultAsync(mutedUserModel);

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
        public async Task<bool> IsUserKickedAsync(string userId, string roomId)
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
        /// Метод проверяет, является ли пользователь участником комнаты
        /// </summary>
        public async Task<bool> IsUserInGroupAsync(string userId, string roomId)
        {
            var members = await memberRepository.GetAsync(new MemberModel { UserId = userId });
            var memberIds = members.Select(m => m.RoomId).ToList();

            return memberIds.Contains(roomId);
        }

        // TODO: дописать логику, последних двух перменных по умолчнию
        public async Task<bool> CommandAccessCheckAsync(User user, List<string> allowedRoles, bool checkOnOwner, string processingUserName)
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