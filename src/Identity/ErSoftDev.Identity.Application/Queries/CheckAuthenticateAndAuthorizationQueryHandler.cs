using ErSoftDev.Framework.Redis;
using ErSoftDev.Identity.Application.Queries;
using ErSoftDev.Identity.Domain.AggregatesModel.OperateAggregate;
using ErSoftDev.Identity.Domain.SeedWorks;
using ErSoftDev.Identity.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ErSoftDev.Identity.Application.Queries
{
    public class
        CheckAuthenticateAndAuthorizationQueryHandler : IRequestHandler<CheckAuthenticateAndAuthorizationQuery, bool>
    {
        private readonly IdentityQueryDbContext _identityQueryDbContext;
        private readonly IRedisService _redisService;

        public CheckAuthenticateAndAuthorizationQueryHandler(IdentityQueryDbContext identityQueryDbContext,
            IRedisService redisService)
        {
            _identityQueryDbContext =
                identityQueryDbContext ?? throw new ArgumentNullException(nameof(identityQueryDbContext));
            _redisService = redisService;
        }

        public async Task<bool> Handle(CheckAuthenticateAndAuthorizationQuery request,
            CancellationToken cancellationToken)
        {
            var userInfo =
                await _identityQueryDbContext.Users.FirstOrDefaultAsync(user =>
                    user.SecurityStampToken == request.SecurityStampToken, cancellationToken);
            if (userInfo is null)
                return false;

            var authorizeFromCache =
                await _redisService.GetAsync<List<Operate>>(CacheKey.UserOperates + ":" + request.SecurityStampToken);
            if (authorizeFromCache != null!)
            {
                if (authorizeFromCache.Any(operate =>
                        string.Equals(operate.Title, request.Operate, StringComparison.CurrentCultureIgnoreCase)))
                    return true;
                return false;
            }

            var authorizeQuery =
                from user in _identityQueryDbContext.Users
                join userRole in _identityQueryDbContext.UserRoles
                    on user.Id equals userRole.UserId into userGrouping
                from userRole in userGrouping
                where user.SecurityStampToken == request.SecurityStampToken && user.IsActive

                join roleOperate in _identityQueryDbContext.RoleOperates
                    on userRole.RoleId equals roleOperate.RoleId into roleOperateGrouping
                from roleOperate in roleOperateGrouping

                join operate in _identityQueryDbContext.Operates
                    on roleOperate.OperateId equals operate.Id into operateGrouping
                from operate in operateGrouping
                select operate;
            var authorize = await authorizeQuery.ToListAsync(cancellationToken);

            await _redisService.AddOrUpdateAsync(CacheKey.UserOperates + ":" + request.SecurityStampToken, authorize,
                TimeSpan.FromMinutes(10));

            return authorize.Count > 0 &&
                   authorize.Any(operate =>
                       string.Equals(operate.Title, request.Operate, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
