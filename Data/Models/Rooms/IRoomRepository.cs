using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.Rooms
{
    public interface IRoomRepository
    {
        IQueryable<Room> Rooms { get; }

        Room FindRoomById(String roomId);

        Room FindRoomByName(String roomName);

        void AddRoom(Room room);

        void EditRoom(Room room);

        void DeleteRoom(String roomId);

        void Save();
    }
}
