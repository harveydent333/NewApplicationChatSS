using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using NewAppChatSS.DAL;
using Microsoft.AspNetCore.Identity;

namespace NewAppChatSS.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;
        private UserRepository userRepository;
        private TypeRoomRepository typeRoomRepository;
        private UserManager<User> _userManager;

        public EFUnitOfWork(ApplicationDbContext dbContext, UserManager<User> manager)
        {
            db = dbContext;
            _userManager = manager;
        }

        public IRepository<User> Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(db, _userManager);
                return userRepository;
            }
        }

        public IRepository<TypeRoom> TypesRooms
        {
            get
            {
                if (typeRoomRepository == null)
                    typeRoomRepository = new TypeRoomRepository(db);
                return typeRoomRepository;
            }
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
