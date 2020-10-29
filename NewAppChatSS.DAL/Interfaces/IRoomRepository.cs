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

        void Create(Room item);

        void Update(Room item);

        void UpdateById(string roomId);

        void Delete(Room item);

        void DeleteById(string roomId);

        void DeleteByName(string roomName);

        void Save();
    }
}

