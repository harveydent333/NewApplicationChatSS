using System;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }

        IRoomRepository Rooms { get; }

        IMessageRepository Messages { get; }

        IMemberRepository Members { get; }

        IMutedUserRepository MutedUsers { get; }

        IKickedOutsRepository KickedOuts { get; }
    }
}
