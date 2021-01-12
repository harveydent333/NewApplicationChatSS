using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NewAppChatSS.Common.CommonHelpers;
using NewAppChatSS.DAL.Configuration;
using NewAppChatSS.DAL.Entities;
using NewAppChatSS.DAL.Initializers;

namespace NewAppChatSS.DAL
{
    public class NewAppChatSSContext : IdentityDbContext<User>
    {
        public NewAppChatSSContext(DbContextOptions<NewAppChatSSContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(RoomConfiguration)));

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
            }

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