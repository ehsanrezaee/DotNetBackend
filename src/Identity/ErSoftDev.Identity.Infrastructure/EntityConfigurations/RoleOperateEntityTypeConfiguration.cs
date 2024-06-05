using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class RoleOperateEntityTypeConfiguration : IEntityTypeConfiguration<RoleOperate>
    {
        public void Configure(EntityTypeBuilder<RoleOperate> builder)
        {
            builder.ToTable("RoleOperates", IdentityDbContext.DefaultSchema);

            builder.HasKey(operate => operate.Id);
            builder.Property(operate => operate.Id).ValueGeneratedNever();
        }
    }
}
