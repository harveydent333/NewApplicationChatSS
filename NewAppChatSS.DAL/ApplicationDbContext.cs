using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using System;
using Microsoft.AspNetCore.Identity;
using NewAppChatSS.DAL.Initializers;
using NewAppChatSS.Common.CommonHelpers;

namespace NewAppChatSS.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<TypeRoom> TypesRooms { get; set; }

        public DbSet<KickedOut> KickedOuts { get; set; }

        public DbSet<MutedUser> MutedUsers { get; set; }

        public DbSet<DictionaryBadWords> DicrtionaryBadWords { get; set; }

        public DbSet<SwearingUser> SwearingUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userId = NewAppChatGuidHelper.GetNewGuid();
            var adminRoleId = NewAppChatGuidHelper.GetNewGuid();

            UserInitializer.Seed(modelBuilder, userId);
            TypeRoomInitializer.Seed(modelBuilder);
            RoomInitializer.Seed(modelBuilder, userId);
            IdentityRoleInitializer.Seed(modelBuilder, adminRoleId);
            IdentityUserRoleInitializer.Seed(modelBuilder, userId, adminRoleId);
            MemberInitializer.Seed(modelBuilder, userId);
        }
    }
}