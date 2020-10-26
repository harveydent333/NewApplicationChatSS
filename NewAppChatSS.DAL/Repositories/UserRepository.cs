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
    public class UserRepository : IRepository<User>
    {
        private ApplicationDbContext dbUserContext;
        private UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<User> manager)
        {
            dbUserContext = context;
            _userManager = manager;
        }

        public IEnumerable<User> GetAll()
        {
            return _userManager.Users.ToList();
        }

        public User Get(String id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Create(User item)
        {
            await _userManager.CreateAsync(item);
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }

        public void Delete(String id)
        {
            throw new NotImplementedException();
        }
    }
}
