using NewAppChatSS.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();

        Task<User> FindById(string id);

        Task<User> FindByEmailAsync(string email);

        Task Create(User item);

        Task Update(User item);

        Task Delete(User item);

        void Save();
    }
}
