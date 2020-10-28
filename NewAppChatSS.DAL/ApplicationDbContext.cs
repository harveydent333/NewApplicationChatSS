using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) 
        {
         //   ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<TypeRoom> TypesRooms { get; set; }
        public DbSet<KickedOut> KickedOuts { get; set; }
        public DbSet<MutedUser> MutedUsers { get; set; }
        public DbSet<DictionaryBadWords> DicrtionaryBadWords { get; set; }
        public DbSet<SwearingUser> SwearingUsers { get; set; }
    }
}
