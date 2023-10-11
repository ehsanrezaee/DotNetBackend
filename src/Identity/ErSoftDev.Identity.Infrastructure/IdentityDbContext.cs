using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.BaseModel;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ErSoftDev.Identity.Infrastructure
{
    public class IdentityDbContext : BaseDbContext
    {
        public const string DefaultSchema = "dbo";

        private readonly IMediator _mediator;
        public IdentityDbContext(DbContextOptions options, IOptions<AppSetting> appSetting, IMediator mediator) : base(
            options, appSetting, mediator)
        {
            _mediator = mediator;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }
        public DbSet<UserLogin> Logins { get; set; }
    }
}
