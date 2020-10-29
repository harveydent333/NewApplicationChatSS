using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Room> GetAll()
        {
            return _context.Rooms.ToList();
        }

        public Room FindById(string id)
        {
            return _context.Rooms.FirstOrDefault(r => r.Id == id);
        }

        public Room FindByName(string roomName)
        {
            return _context.Rooms.FirstOrDefault(r => r.RoomName == roomName);
        }

        public void Create(Room item)
        {
            _context.Rooms.Add(item);
            Save();
        }

        public void Update(Room room)
        {
            _context.Rooms.Update(room);
            Save();
        }

        public void UpdateById(string roomId)
        {
            _context.Rooms.Update(FindById(roomId));
            Save();
        }

        public void Delete(Room room)
        {
            _context.Rooms.Remove(room);
            Save();
        }

        public void DeleteById(string roomId)
        {
            _context.Rooms.Remove(FindById(roomId));
            Save();
        }

        public void DeleteByName(string roomName)
        {
            _context.Rooms.Remove(FindByName(roomName));
            Save();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
