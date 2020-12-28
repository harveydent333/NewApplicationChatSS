using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Interfaces;

namespace NewAppChatSS.DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;
        private bool disposed = false;

        private RoomRepository roomRepository;
        private IMemberRepository memberRepository;
        private IMessageRepository messageRepository;
        private IMutedUserRepository mutedUserRepository;
        private IKickedOutRepository kickedOutsRepository;

        public EFUnitOfWork(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public IUnitOfWork Database { get; set; }

        public IKickedOutRepository KickedOuts
        {
            get
            {
                if (kickedOutsRepository == null)
                {
                    kickedOutsRepository = new KickedOutRepository(db);
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
