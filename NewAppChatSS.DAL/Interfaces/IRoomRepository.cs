using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IRoomRepository
    {
        IEnumerable<Room> GetAll();

        Room FindById(string id);

        Room FindByName(string roomName);

        Task CreateAsync(Room item);

        Task UpdateAsync(Room item);

        Task UpdateByIdAsync(string roomId);

        Task DeleteAsync(Room item);

        Task DeleteByIdAsync(string roomId);

        Task DeleteByNameAsync(string roomName);

        Task SaveAsync();
    }
}

