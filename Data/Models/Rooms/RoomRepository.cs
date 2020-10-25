using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Data.Models.Rooms
{
    public class RoomRepository : IRoomRepository
    {
        private ApplicationDbContext roomContext;

        public RoomRepository(ApplicationDbContext context)
        {
            roomContext = context;
        }

        public IQueryable<Room> Rooms => roomContext.Rooms;

        /// <summary>
        /// Метод добавляет запись комнаты в базу данных
        /// </summary>
        public void AddRoom(Room room)
        {
            roomContext.Rooms.Add(room);
            Save();
        }

        /// <summary>
        /// Метод удаляет запись комнаты в базе данных
        /// </summary>
        public void DeleteRoom(String roomId)
        {
            roomContext.Remove(FindRoomById(roomId));
            Save();
        }

        /// <summary>
        /// Метод изменяет запись комнаты в базе данных
        /// </summary>
        public void EditRoom(Room room)
        {
            roomContext.Rooms.Update(room);
            Save();
        }

        /// <summary>
        /// Метод ищет запись комнаты в базе данных по  id комнаты
        /// </summary>
        public Room FindRoomById(String roomId)
        {
            return roomContext.Rooms
                .Include(t => t.TypeRoom)
                .Where(r => r.Id == roomId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Метод ищет запись комнаты в базе данных по названию комнаты
        /// </summary>
        public Room FindRoomByName(String roomName)
        {
            return roomContext.Rooms
                .Include(t => t.TypeRoom)
                .Where(r => r.RoomName == roomName)
                .FirstOrDefault();
        }

        /// <summary>
        /// Метод сохраняет состояние записей в базе данных
        /// </summary>
        public void Save()
        {
            roomContext.SaveChanges();
        }
    }
}
