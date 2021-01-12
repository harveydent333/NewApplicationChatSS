using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class SwearingUserConfiguration : BaseConfiguration<SwearingUser, int>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<SwearingUser> builder)
        {
            builder.ToTable("swaring_users");

            builder
                .HasOne(k => k.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);
        }
    }
}
