using AppChatSS.Infrastucture;
using AppChatSS.Models.KickedOuts;
using AppChatSS.Models.Members;
using AppChatSS.Models.MutedUsers;
using AppChatSS.Models.Users;
using AppChatSS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppChatSS.Validation
{
    public class UserValidator
    {
        private static IUserRepository userRepository;
        private static IMemberRepository memberRepository;
        private static IKickedOutsRepository kickedOutsRepository;
        private static IMutedUserRepository mutedUserRepository;

        public UserValidator(IUserRepository userRep, IMemberRepository memberRep, IKickedOutsRepository kickedOutsRep, IMutedUserRepository mutedUserRep)
        {
            userRepository = userRep;
            memberRepository = memberRep;
            kickedOutsRepository = kickedOutsRep;
            mutedUserRepository = mutedUserRep;
        }

        /// <summary>
        /// Метод проверяет на уникальность значение логина
        /// </summary>
        /// <returns>Возвращает True если логин уникальный</returns>
        public static Boolean UniquenessCheckUser(String userLogin)
        {
            return userRepository.FindUserByLogin(userLogin) == null;
        }

        /// <summary>
        /// Метод проверяет существует ли в базе данных запись с логином и паролем, переданными авторизируемым пользователем
        /// </summary>
        /// <returns>Возвращает True если запись есть в таблице базы данных </returns>
        public static Boolean PresenceCheckUser(LoginModel loginModel)
        {
            var password = HashPassword.GetHashPassword(loginModel.Password);

            User user = userRepository.Users
                .FirstOrDefault(u => u.Login == loginModel.Login && u.Password == password);

            return (user != null) ? true : false;
        }

        /// <summary>
        /// Метод проверяет заблокирован ли пользователь в приложении
        /// </summary>
        public static Boolean IsUserBlocked(User user)
        {
            if (user.Loked && (DateTime.Now > user.DateUnblock))
            {
                user.Loked = false;
                userRepository.EditUser(user);
            }

            return user.Loked;
        }

        /// <summary>
        /// Метод проверяет имеет ли пользователь возможность отправлять сообщения в чат в комнате
        /// </summary>
        public static Boolean IsUserMuted(Int32? userId, String roomId)
        {
            List<MutedUser> listRoomWhereMutedUser = mutedUserRepository.GetListMutedRoomForUser(userId);
            MutedUser mutedUser = listRoomWhereMutedUser
                .Where(m => m.RoomId == roomId)
                .FirstOrDefault();

            if (mutedUser != null)
            {
                if(mutedUser.DateUnmute > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    mutedUserRepository.DeleteMutedUser(userId, roomId);
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
        public static Boolean IsUserKicked(Int32? userId, String roomId)
        {
            List<KickedOut> listKickedOut = kickedOutsRepository.GetListKickedRoomForUser(userId);

            KickedOut kicked = listKickedOut
                .Where(k => k.RoomId == roomId)
                .FirstOrDefault();

            if (kicked != null)
            {
                if (kicked.DateUnkick > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    kickedOutsRepository.DeleteKickedUser(userId, roomId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// /// Проверка возможности использовать команду пользователем.
        /// </summary>
        public static Boolean CommandAccessCheck(User user, IEnumerable<String> allowedRoles, Boolean checkOnOwner = false, String processingLogin = "")
        {
            if (allowedRoles.Contains(user.Role.RoleName))
            {
                return true;
            }

            if (checkOnOwner)
            {
                return IsOwnerLogin(user.Login, processingLogin);
            }

            return false;
        }

        /// <summary>
        /// Проверка на владелеца логина. 
        /// Принимает логин пользователя который отправлял команду и логин из команды, сравнивает их
        /// </summary>
        /// <returns>Возвращает True если два логина совпадают.</returns>
        public static Boolean IsOwnerLogin(String ownerLogin, String processingLogin)
        {
            if (ownerLogin == processingLogin)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет является ли пользователь участником комнаты
        /// </summary>
        public static Boolean IsUserInGroup(Int32? userId, String roomId)
        {
            return memberRepository.GetRooms(userId).Contains(roomId);
        }
    }
}