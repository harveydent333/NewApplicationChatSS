using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class MemberConfiguration : BaseConfiguration<Member, int>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("members");

            builder
                .HasOne(m => m.Room)
                .WithMany()
                .HasForeignKey(f => f.RoomId);

            builder
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);
        }
    }
}
