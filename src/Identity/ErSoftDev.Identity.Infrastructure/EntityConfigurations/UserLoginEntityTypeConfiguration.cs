using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class UserLoginEntityTypeConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogins", IdentityDbContext.DefaultSchema);

            builder.HasKey(login => login.Id);
            builder.Property(login => login.Id).ValueGeneratedNever();
            builder.Property(login => login.UserId).IsRequired();
            builder.Property(login => login.Browser).HasMaxLength(100).IsRequired(false);
            builder.Property(login => login.DeviceName).HasMaxLength(100).IsRequired(false);
            builder.Property(login => login.DeviceUniqueId).HasMaxLength(100).IsRequired(false);
            builder.Property(login => login.FcmToken).HasMaxLength(100).IsRequired(false);
        }
    }
}
