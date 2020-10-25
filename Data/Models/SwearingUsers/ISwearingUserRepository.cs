using System;
using System.Linq;

namespace Data.Models.SwearingUsers
{
    public interface ISwearingUserRepository
    {
        IQueryable<SwearingUser> SwearingUsers { get; }

        void AddSwearingUser(SwearingUser swearingUse);

        void EditSwearingUser(SwearingUser swearingUser);

        SwearingUser FindSwearingUserByUserId(Int32? userId);

        void Save();
    }
}
