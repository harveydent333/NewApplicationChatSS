using System;
using System.Text.Json;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

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
        public async Task<string> CreateRoom(string roomName, int typeRoomId, string userId)
        {
            string roomId = Guid.NewGuid().ToString();

            Room newRoom = new Room
            {
                Id = roomId,
                RoomName = roomName,
                OwnerId = userId,
                TypeId = typeRoomId
            };

            await Database.Rooms.CreateAsync(newRoom);
            await Database.Members.AddMemberAsync(userId, roomId);

            return JsonSerializer.Serialize<object>(
                new
                {
                    roomId = newRoom.Id,
                    roomName = newRoom.RoomName
                });
        }
    }
}
