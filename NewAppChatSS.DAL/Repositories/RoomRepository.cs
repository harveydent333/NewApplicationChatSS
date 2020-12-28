using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.DAL.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext context;

        public RoomRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<Room> GetAll()
        {
            return context.Rooms
                .Include(r => r.Owner)
                .Include(r => r.LastMessage)
                .Include(r => r.TypeRoom)
                .ToList();
        }

        public Room FindById(string id)
        {
            return context.Rooms
                .Include(r => r.Owner)
                .Include(r => r.LastMessage)
                .Include(r => r.TypeRoom)
                .FirstOrDefault(r => r.Id == id);
        }

        public Room FindByName(string roomName)
        {
            return context.Rooms
                .Include(r => r.Owner)
                .Include(r => r.LastMessage)
                .Include(r => r.TypeRoom)
                .FirstOrDefault(r => r.RoomName == roomName);
        }

        public async Task CreateAsync(Room item)
        {
            context.Rooms.Add(item);
            await SaveAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            context.Rooms.Update(room);
            await SaveAsync();
        }

        public async Task UpdateByIdAsync(string roomId)
        {
            context.Rooms.Update(FindById(roomId));
            await SaveAsync();
        }

        public async Task DeleteAsync(Room room)
        {
            context.Rooms.Remove(room);
            await SaveAsync();
        }

        public async Task DeleteByIdAsync(string roomId)
        {
            context.Rooms.Remove(FindById(roomId));
            await SaveAsync();
        }

        public async Task DeleteByNameAsync(string roomName)
        {
            context.Rooms.Remove(FindByName(roomName));
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
