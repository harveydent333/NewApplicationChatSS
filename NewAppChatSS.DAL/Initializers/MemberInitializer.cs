using Microsoft.EntityFrameworkCore;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Initializers
{
    public static class MemberInitializer
    {
        public static void Seed(ModelBuilder modelBuilder, string userId)
        {
            modelBuilder.Entity<Member>().HasData(
                new Member()
                {
                    Id = 1,
                    UserId = userId,
                    RoomId = GlobalConstants.MainRoomId,
                });
        }
    }
}
