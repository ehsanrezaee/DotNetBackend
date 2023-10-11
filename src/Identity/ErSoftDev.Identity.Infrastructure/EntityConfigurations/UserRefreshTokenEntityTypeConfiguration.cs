using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErSoftDev.Identity.Infrastructure.EntityConfigurations
{
    public class UserRefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.ToTable("UserRefreshTokens", IdentityDbContext.DefaultSchema);

            builder.HasKey(token => token.Id);
            builder.Property(token => token.Id).ValueGeneratedNever();
            builder.Property(token => token.ExpireAt).IsRequired();
            builder.Property(token => token.IsActive).IsRequired();
            builder.Property(token => token.IsRevoke).IsRequired();
            builder.Property(token => token.IsUse).IsRequired();
            builder.Property(token => token.UserId).IsRequired();
        }
    }
}
