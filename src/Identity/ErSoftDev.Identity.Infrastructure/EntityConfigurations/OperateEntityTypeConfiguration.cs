using ErSoftDev.Identity.Domain.AggregatesModel.OperateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class OperateEntityTypeConfiguration : IEntityTypeConfiguration<Operate>
    {
        public void Configure(EntityTypeBuilder<Operate> builder)
        {
            builder.ToTable("Operates", IdentityDbContext.DefaultSchema);

            builder.HasKey(operate => operate.Id);
            builder.Property(operate => operate.Id).ValueGeneratedNever();
            builder.Property(operate => operate.Title).HasColumnType("nvarchar").HasMaxLength(100).IsRequired();
            builder.Property(operate => operate.Description).HasColumnType("nvarchar").HasMaxLength(500).IsRequired();
        }
    }
}
