using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppChatSS.Models.SwearingUsers
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
