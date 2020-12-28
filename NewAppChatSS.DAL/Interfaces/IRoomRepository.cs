using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Repositories.Models;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IRoomRepository : IBaseRepository<Room, string, ApplicationDbContext, RoomModel>
    {
        List<Room> GetAll();

        Room FindById(string id);

        Room FindByName(string roomName);

        Task CreateAsync(Room item);

        Task UpdateAsync(Room item);

        Task UpdateByIdAsync(string roomId);

        Task DeleteAsync2(Room item);

        Task DeleteByIdAsync(string roomId);

        Task DeleteByNameAsync(string roomName);

        Task SaveAsync();
    }
}