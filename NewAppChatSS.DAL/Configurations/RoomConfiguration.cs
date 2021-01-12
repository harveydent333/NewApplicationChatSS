using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class RoomConfiguration : BaseConfiguration<Room, string>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("rooms");

            builder
                .HasOne(k => k.TypeRoom)
                .WithMany()
                .HasForeignKey(f => f.TypeId);

            builder
                .HasOne(k => k.LastMessage)
                .WithMany()
                .HasForeignKey(f => f.LastMessageId);

            builder
                .HasOne(k => k.Owner)
                .WithMany()
                .HasForeignKey(f => f.OwnerId);
        }
    }
}
