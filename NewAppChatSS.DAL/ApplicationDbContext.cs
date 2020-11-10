using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using System;
using Microsoft.AspNetCore.Identity;

namespace NewAppChatSS.DAL
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        const string MAIN_ROOM_ID = "1";

        private readonly string _userId;
        private readonly string _adminRoleId;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
            _userId = Guid.NewGuid().ToString();
            _adminRoleId = Guid.NewGuid().ToString();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = _userId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = false,
                PasswordHash = "AQAAAAEAACcQAAAAEPp5IfNNpKxmZneDEq+E9JlFLrCvuZVewUvPr/gyAhxLdouVInuOzQUdc8fPbhJvlg==",
                SecurityStamp = string.Empty,
                ConcurrencyStamp = "af10709c-ab87-4c8a-badc-2d1e0421adfa",
                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                IsLocked = false,
                DateUnblock = null
            });

            modelBuilder.Entity<TypeRoom>().HasData(new TypeRoom
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

            modelBuilder.Entity<Room>().HasData(new Room
            {
                Id = "1",
                RoomName = "MainRoom",
                OwnerId = _userId,
                TypeId = 1,
            });

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "RegularUser",
                NormalizedName = "REGULARUSER",
                ConcurrencyStamp = "58b46a8b-f923-48df-bbd2-ae752ceea327"
            },

            new IdentityRole
            {
                Id = _adminRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "683023fe-4cb7-4d82-9e82-adbe9def1222"
            },

            new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                ConcurrencyStamp = "fa4da5b5-ad1a-44d7-8ba1-130b55a434c8"
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = _userId,
                RoleId = _adminRoleId
            });

            modelBuilder.Entity<Member>().HasData(new Member
            {
                Id = 1,
                UserId = _userId,
                RoomId = MAIN_ROOM_ID
            });
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