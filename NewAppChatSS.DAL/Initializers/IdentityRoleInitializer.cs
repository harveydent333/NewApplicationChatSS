using System;
using NewAppChatSS.Common.CommonHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NewAppChatSS.DAL.Initializers
{
    public static class IdentityRoleInitializer
    {
        public static void Seed(ModelBuilder modelBuilder, string adminRoleId)
        {
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole()
            {
                Id = NewAppChatGuidHelper.GetNewGuid(),
                Name = "RegularUser",
                NormalizedName = "REGULARUSER",
                ConcurrencyStamp = "58b46a8b-f923-48df-bbd2-ae752ceea327"
            },

            new IdentityRole()
            {
                Id = adminRoleId,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                ConcurrencyStamp = "683023fe-4cb7-4d82-9e82-adbe9def1222"
            },

            new IdentityRole()
            {
                Id = NewAppChatGuidHelper.GetNewGuid(),
                Name = "Moderator",
                NormalizedName = "MODERATOR",
                ConcurrencyStamp = "fa4da5b5-ad1a-44d7-8ba1-130b55a434c8"
            });
        }
    }
}
