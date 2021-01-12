using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class MessageConfiguration : BaseConfiguration<Message, string>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("messages");

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
