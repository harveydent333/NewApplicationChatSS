using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Interfaces
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(String id);
        IEnumerable<T> Find(Func<T, Boolean> predicate);
        void Create(T item);
        void Update(T item);
        void Delete(String id);
    }
}
