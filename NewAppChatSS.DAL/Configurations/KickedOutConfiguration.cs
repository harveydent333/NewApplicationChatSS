using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class KickedOutConfiguration : BaseConfiguration<KickedOut, int>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<KickedOut> builder)
        {
            builder.ToTable("kicked_outs");

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
