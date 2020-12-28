using System;

namespace NewAppChatSS.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomRepository Rooms { get; }

        IMessageRepository Messages { get; }

        IMemberRepository Members { get; }

        IMutedUserRepository MutedUsers { get; }

        IKickedOutRepository KickedOuts { get; }
    }
}
