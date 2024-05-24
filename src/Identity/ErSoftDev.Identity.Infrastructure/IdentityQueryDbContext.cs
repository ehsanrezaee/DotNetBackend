using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.BaseModel;
using ErSoftDev.Identity.Domain.AggregatesModel.OperateAggregate;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ErSoftDev.Identity.Infrastructure
{
    public class IdentityQueryDbContext : BaseDbContext
    {
        public IdentityQueryDbContext(DbContextOptions<IdentityQueryDbContext> options, IOptions<AppSetting> appSetting,
            IMediator mediator, IHttpContextAccessor httpContextAccessor) : base(options, appSetting, mediator
            , httpContextAccessor)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }
        public DbSet<RoleOperate> RoleOperates { get; set; }
        public DbSet<Operate> Operates { get; set; }
    }
}
