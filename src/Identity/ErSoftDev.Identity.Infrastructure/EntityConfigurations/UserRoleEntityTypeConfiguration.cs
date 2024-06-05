using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles", IdentityDbContext.DefaultSchema);

            builder.HasKey(role => role.Id);
            builder.Property(role => role.Id).ValueGeneratedNever();
        }
    }
}
