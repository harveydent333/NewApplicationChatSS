using Microsoft.EntityFrameworkCore;
using NewAppChatSS.Common.Constants;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Initializers
{
    public static class RoomInitializer
    {
        public static void Seed(ModelBuilder modelBuilder, string userId)
        {
            modelBuilder.Entity<Room>().HasData(
                new Room()
                {
                    Id = GlobalConstants.MainRoomId,
                    RoomName = "MainRoom",
                    OwnerId = userId,
                    TypeId = 1,
                });
        }
    }
}
