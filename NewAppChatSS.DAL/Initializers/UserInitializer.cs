using Microsoft.EntityFrameworkCore;
using NewAppChatSS.DAL.Entities;
using System;

namespace NewAppChatSS.DAL.Initializers
{
    public static class UserInitializer
    {
        public static void Seed(ModelBuilder modelBuilder, string userId)
        {
            modelBuilder.Entity<User>().HasData(new User()
            {
                Id = userId,
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
        }
    }
}
