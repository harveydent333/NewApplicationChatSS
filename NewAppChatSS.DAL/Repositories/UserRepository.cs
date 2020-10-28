using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace NewAppChatSS.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbUserContext;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<User> manager)
        {
            _dbUserContext = context;
            _userManager = manager;
        }

        public IEnumerable<User> GetAll()
        {
            return _dbUserContext.Users.ToList();
        }

        public User FindById(string id)
        {
            return _dbUserContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public User FindByEmail(string email)
        {
            return _dbUserContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public async Task Create(User item)
        {
            item.Id = Guid.NewGuid().ToString();
            await _userManager.CreateAsync(item, item.PasswordHash);
        }

        public async Task Update(User item)
        {
            await _userManager.UpdateAsync(item);
        }

        public async Task Delete(User item)
        {
            await _userManager.DeleteAsync(item);
        }

    }
}
