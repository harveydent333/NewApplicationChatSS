using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewAppChatSS.DAL.Entities;

namespace NewAppChatSS.DAL.Configuration
{
    public abstract class BaseConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : EntityBase<TKey>
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(p => p.Id);

            ConfigureCustom(builder);
        }

        protected abstract void ConfigureCustom(EntityTypeBuilder<TEntity> builder);
    }
}
