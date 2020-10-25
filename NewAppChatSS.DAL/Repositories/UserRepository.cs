using NewAppChatSS.DAL.Interfaces;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewAppChatSS.DAL.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private ApplicationDbContext dbUserContext;

        UserRepository(ApplicationDbContext context)
        {
            dbUserContext = context;
        }

        public IEnumerable<User> GetAll()
        {
            return dbUserContext.Users;
        }

        public User Get(String id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Create(User item)
        {
            throw new NotImplementedException();
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
