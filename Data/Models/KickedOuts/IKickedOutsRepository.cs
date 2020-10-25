using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Models.KickedOuts
{
    public interface IKickedOutsRepository
    {
        IQueryable<KickedOut> KickedOuts { get; }

        void AddKickeddUser(Int32? userId, String roomId, DateTime dateUnkick);

        void DeleteKickedUser(Int32? userId, String roomId);

        List<KickedOut> GetListKickedRoomForUser(Int32? userId);

        void Save();

    }
}
