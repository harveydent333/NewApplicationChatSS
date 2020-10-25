using System;
using NewAppChatSS.DAL.Entities;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    interface IUnitOfWork : IDisposable
    {
     //   IRepository<User> Users { get; }
        void Save();
    }
}
