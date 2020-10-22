using AppChatSS.Models.Rooms;
using AppChatSS.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppChatSS.Infrastucture
{
    public class RoomValidator
    {
        private static IRoomRepository roomRepository;

        public RoomValidator(IRoomRepository roomRep)
        {
            roomRepository = roomRep;
        }

        /// <summary>
        /// Метод проверяет есть ли в базе данных запись с таким же именем комнаты
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns>Возвращает True если записей нет и False если уже присутсвуют</returns>
        public static Boolean UniquenessCheckRoom(String roomName)
        {
            return roomRepository.FindRoomByName(roomName) == null;
        }

        /// <summary>
        /// Метод проверяет доступ к командам взаимодействия с комнатой у пользователя
        /// </summary>
        public static Boolean CommandAccessCheck(User user, IEnumerable<String> allowedRoles, String nameProcessedRoom)
        {
            if (IsOwnerRoom(user.Id, nameProcessedRoom))
            {
                return true;
            }

            if (allowedRoles.Contains(user.Role.RoleName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Метод проверяет является ли пользователь владельцем комнаты
        /// </summary>
        public static Boolean IsOwnerRoom(Int32? userId, String nameProcessedRoom)
        {
            if (roomRepository.FindRoomByName(nameProcessedRoom).OwnerId == userId)
            {
                return true;
            }

            return false;
        }

    }
}
