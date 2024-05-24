using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", IdentityDbContext.DefaultSchema);

            builder.HasKey(role => role.Id);
            builder.Property(role => role.Title).HasMaxLength(100).IsRequired();
            builder.Property(role => role.Description).HasMaxLength(500).IsRequired(false);

            builder.HasMany(role => role.RoleOperates).WithOne(userRole => userRole.Role)
                .HasForeignKey(userRole => userRole.RoleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
