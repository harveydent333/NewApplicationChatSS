using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public class TypeRoomConfiguration : BaseConfiguration<TypeRoom, int>
    {
        protected override void ConfigureCustom(EntityTypeBuilder<TypeRoom> builder)
        {
            builder.ToTable("type_rooms");
        }
    }
}
