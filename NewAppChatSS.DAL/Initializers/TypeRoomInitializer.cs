using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Initializers
{
    public static class TypeRoomInitializer
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeRoom>().HasData(new TypeRoom()
            {
                Id = 1,
                TypeName = "RegularRoom",
            },

            new TypeRoom
            {
                Id = 2,
                TypeName = "PrivateRoom",
            },

            new TypeRoom
            {
                Id = 3,
                TypeName = "BotRoom",
            });
        }
    }
}
