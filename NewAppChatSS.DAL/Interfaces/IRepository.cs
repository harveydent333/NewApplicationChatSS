using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(String id);
        IEnumerable<T> Find(Func<T, Boolean> predicate);
        Task Create(T item);
        void Update(T item);
        void Delete(String id);
    }
}
