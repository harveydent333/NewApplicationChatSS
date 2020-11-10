﻿using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace NewAppChatSS.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        private readonly ApplicationDbContext db;
        private RoleRepository roleRepository;
        private RoomRepository roomRepository;
        private UserRepository userRepository;
        private IMemberRepository memberRepository;
        private IMessageRepository messageRepository;
        private IMutedUserRepository mutedUserRepository;
        private IKickedOutsRepository kickedOutsRepository;
        private readonly UserManager<User> _userManager;
        public IUnitOfWork Database { get; set; }

        public EFUnitOfWork(ApplicationDbContext dbContext, UserManager<User> manager)
        {
            db = dbContext;
            _userManager = manager;
        }

        public IKickedOutsRepository KickedOuts
        {
            get
            {
                if (kickedOutsRepository == null)
                {
                    kickedOutsRepository = new KickedOutsRepository(db);
                }
                return kickedOutsRepository;
            }
        }

        public IMutedUserRepository MutedUsers
        {
            get
            {
                if (mutedUserRepository == null)
                {
                    mutedUserRepository = new MutedUserRepository(db);
                }
                return mutedUserRepository;
            }
        }

        public IMessageRepository Messages
        {
            get
            {
                if (messageRepository == null)
                {
                    messageRepository = new MessageRepository(db);
                }
                return messageRepository;
            }
        }

        public IMemberRepository Members
        {
            get
            {
                if (memberRepository == null)
                {
                    memberRepository = new MemberRepository(db, this);
                }
                return memberRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository = new UserRepository(db, _userManager);
                }
                return userRepository;
            }
        }

        public IRoomRepository Rooms
        {
            get
            {
                if (roomRepository == null)
                {
                    roomRepository = new RoomRepository(db);
                }
                return roomRepository;
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                if (roleRepository == null)
                {
                    roleRepository = new RoleRepository(db);
                }
                return roleRepository;
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
