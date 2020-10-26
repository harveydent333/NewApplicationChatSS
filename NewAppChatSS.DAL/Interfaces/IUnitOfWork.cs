using System;
using NewAppChatSS.DAL.Entities;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IRepository<TypeRoom> TypesRooms { get; }
        void Save();
    }
}
