using NewAppChatSS.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();

        Task<User> FindById(string id);

        Task<User> FindByEmailAsync(string email);

        Task CreateAsync(User item);

        Task UpdateAsync(User item);

        Task DeleteAsync(User item);

        Task SaveAsync();
    }
}
