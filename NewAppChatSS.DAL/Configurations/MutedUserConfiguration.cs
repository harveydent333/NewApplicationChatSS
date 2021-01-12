using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class MutedUserConfiguration : BaseConfiguration<MutedUser, int>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<MutedUser> builder)
        {
            builder.ToTable("muted_users");

            builder
                .HasOne(k => k.Room)
                .WithMany()
                .HasForeignKey(f => f.RoomId);

            builder
                .HasOne(k => k.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);
        }
    }
}
