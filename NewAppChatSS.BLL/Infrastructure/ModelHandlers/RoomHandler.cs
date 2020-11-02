using Microsoft.EntityFrameworkCore.Storage;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Text.Json;

namespace NewAppChatSS.BLL.Infrastructure.ModelHandlers
{
    public class RoomHandler : IRoomHandler
    {
        public IUnitOfWork Database { get; set; }

        public RoomHandler(IUnitOfWork uow)
        {
            Database = uow;
        }

        /// <summary>
        /// Метод создает новую комнату 
        /// </summary>
        public string CreateRoom(string roomName, int typeRoomId, string userId)
        {
            string roomId = Guid.NewGuid().ToString();

            Room newRoom = new Room
            {
                Id = roomId,
                RoomName = roomName,
                OwnerId = userId,
                TypeId = typeRoomId
            };

            Database.Rooms.Create(newRoom);
            Database.Members.AddMember(userId, roomId);

            return JsonSerializer.Serialize<object>(
                new
                {
                    roomId = newRoom.Id,
                    roomName = newRoom.RoomName
                });
        }
    }
}
