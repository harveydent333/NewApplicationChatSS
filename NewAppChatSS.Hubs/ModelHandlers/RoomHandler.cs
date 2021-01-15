using System.Text.Json;
using System.Threading.Tasks;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.Hubs.Interfaces.ModelHandlerInterfaces;

namespace NewAppChatSS.Hubs.ModelHandlers
{
    public class RoomHandler : IRoomHandler
    {
        private readonly IRoomRepository roomRepository;
        private readonly IMemberRepository memberRepository;

        public RoomHandler(
            IMemberRepository memberRepository,
            IRoomRepository roomRepository)
        {
            this.memberRepository = memberRepository;
            this.roomRepository = roomRepository;
        }

        /// <summary>
        /// Метод создает новую комнату
        /// </summary>
        public async Task<string> CreateRoomAsync(string roomName, int typeRoomId, string userId)
        {
            string roomId = NewAppChatGuidHelper.GetNewGuid();

            Room newRoom = new Room
            {
                Id = roomId,
                RoomName = roomName,
                OwnerId = userId,
                TypeId = typeRoomId
            };

            await roomRepository.AddAsync(newRoom);

            var member = new Member
            {
                UserId = userId,
                RoomId = roomId,
            };

            await memberRepository.AddAsync(member);

            var roomInfo = new
            {
                roomId = newRoom.Id,
                roomName = newRoom.RoomName
            };

            return JsonSerializer.Serialize<object>(roomInfo);
        }
    }
}
