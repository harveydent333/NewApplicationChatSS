using System.Text.Json;
using System.Threading.Tasks;
using NewAppChatSS.BLL.Interfaces.ModelHandlerInterfaces;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.BLL.Infrastructure.ModelHandlers
{
    public class RoomHandler : IRoomHandler
    {
        public RoomHandler(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IUnitOfWork Database { get; set; }

        /// <summary>
        /// Метод создает новую комнату
        /// </summary>
        public async Task<string> CreateRoom(string roomName, int typeRoomId, string userId)
        {
            string roomId = NewAppChatGuidHelper.GetNewGuid();

            Room newRoom = new Room
            {
                Id = roomId,
                RoomName = roomName,
                OwnerId = userId,
                TypeId = typeRoomId
            };

            await Database.Rooms.CreateAsync(newRoom);
            await Database.Members.AddMemberAsync(userId, roomId);

            var roomInfo = new
            {
                roomId = newRoom.Id,
                roomName = newRoom.RoomName
            };

            return JsonSerializer.Serialize<object>(roomInfo);
        }
    }
}
