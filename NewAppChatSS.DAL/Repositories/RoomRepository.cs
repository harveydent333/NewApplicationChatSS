using Microsoft.EntityFrameworkCore;
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
            return _context.Rooms
                .Include(r => r.Owner)
                .Include(r => r.LastMessage)
                .Include(r => r.TypeRoom)
                .ToList();
        }

        public Room FindById(string id)
        {
            return _context.Rooms
                .Include(r => r.Owner)
                .Include(r => r.LastMessage)
                .Include(r => r.TypeRoom)
                .FirstOrDefault(r => r.Id == id);
        }

        public Room FindByName(string roomName)
        {
            return _context.Rooms
                .Include(r => r.Owner)
                .Include(r => r.LastMessage)
                .Include(r => r.TypeRoom)
                .FirstOrDefault(r => r.RoomName == roomName);
        }

        public async Task CreateAsync(Room item)
        {
            _context.Rooms.Add(item);
            await SaveAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await SaveAsync();
        }

        public async Task UpdateByIdAsync(string roomId)
        {
            _context.Rooms.Update(FindById(roomId));
            await SaveAsync();
        }

        public async Task DeleteAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await SaveAsync();
        }

        public async Task DeleteByIdAsync(string roomId)
        {
            _context.Rooms.Remove(FindById(roomId));
            await SaveAsync();
        }

        public async Task DeleteByNameAsync(string roomName)
        {
            _context.Rooms.Remove(FindByName(roomName));
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
