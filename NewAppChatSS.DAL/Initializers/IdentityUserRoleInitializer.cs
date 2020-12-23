using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NewAppChatSS.DAL.Initializers
{
    public static class IdentityUserRoleInitializer
    {
        public static void Seed(ModelBuilder modelBuilder, string userId, string adminRoleId)
        {
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
            {
                UserId = userId,
                RoleId = adminRoleId,
            });
        }
    }
}
