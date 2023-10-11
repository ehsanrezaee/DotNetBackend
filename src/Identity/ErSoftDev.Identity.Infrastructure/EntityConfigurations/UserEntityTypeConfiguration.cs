using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", IdentityDbContext.DefaultSchema);

            builder.HasKey(user => user.Id);
            builder.Property(user => user.Id).ValueGeneratedNever();
            builder.Property(user => user.Firstname).HasMaxLength(50).HasColumnType("nvarchar").IsRequired();
            builder.Property(user => user.Lastname).HasMaxLength(50).HasColumnType("nvarchar").IsRequired();
            builder.Property(user => user.Username).HasMaxLength(50).IsRequired();
            builder.Property(user => user.Password).HasMaxLength(200).IsRequired();
            builder.Property(user => user.CellPhone).HasMaxLength(20).IsRequired(false);
            builder.Property(user => user.Email).HasMaxLength(100).IsRequired(false);
            builder.Property(user => user.Image).HasMaxLength(500).IsRequired(false);
            builder.Property(user => user.SecurityStampToken).HasMaxLength(36).IsRequired(false);

            builder.OwnsOne(user => user.Address,
                cb =>
                {
                    cb.Property(address => address.AddressLine).HasColumnName(nameof(Address.AddressLine))
                        .HasMaxLength(500).HasColumnType("nvarchar").IsRequired(false);
                    cb.Property(address => address.Plaque).HasColumnName(nameof(Address.Plaque))
                        .HasMaxLength(20).HasColumnType("nvarchar").IsRequired(false);
                    cb.Property(address => address.PostalCode).HasColumnName(nameof(Address.PostalCode))
                        .HasMaxLength(10).HasColumnType("varchar").IsRequired(false);
                    cb.Property(address => address.Unit).HasColumnName(nameof(Address.Unit))
                        .HasMaxLength(20).HasColumnType("nvarchar").IsRequired(false);
                });

            builder.HasMany(user => user.UserRoles).WithOne(role => role.User).HasForeignKey(role => role.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(user => user.UserLogins).WithOne(login => login.User).HasForeignKey(login => login.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(user => user.UserRefreshTokens).WithOne(token => token.User).HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
