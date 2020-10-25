using Data.Models.Members;
using Data.Models.Rooms;
using System;
using System.Text.Json;

namespace Infrastructure.ModelHandlers
{
    public class RoomHandler
    {
        private static IRoomRepository roomRepository;
        private static IMemberRepository memberRepository;

        public RoomHandler(IRoomRepository roomRep, IMemberRepository memberRep)
        {
            roomRepository = roomRep;
            memberRepository = memberRep;
        }

        /// <summary>
        /// Метод создает новую комнату 
        /// </summary>
        public static String CreateRoom(String roomName, Int32 typeRoomId, Int32 userId)
        {
            String roomId = Guid.NewGuid().ToString();

            Room newRoom = new Room
            {
                Id = roomId,
                RoomName = roomName,
                OwnerId = userId,
                TypeId = typeRoomId
            };

            roomRepository.AddRoom(newRoom);
            memberRepository.AddMember(userId, roomId);

            var roomInfo = new
            {
                roomId = newRoom.Id,
                roomName = newRoom.RoomName
            };

            return JsonSerializer.Serialize<object>(roomInfo);
        }
    }
}
