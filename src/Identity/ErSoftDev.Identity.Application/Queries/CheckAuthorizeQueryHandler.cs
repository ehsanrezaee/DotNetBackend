using ErSoftDev.Framework.Redis;
using ErSoftDev.Identity.Application.Queries;
using ErSoftDev.Identity.Domain.AggregatesModel.OperateAggregate;
using ErSoftDev.Identity.Domain.SeedWorks;
using ErSoftDev.Identity.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ErSoftDev.Identity.Application.Queries
{
    public class CheckAuthorizeQueryHandler : IRequestHandler<CheckAuthorizeQuery, bool>
    {
        private readonly IRedisService _redisService;
        private readonly IdentityQueryDbContext _identityQueryDbContext;

        public CheckAuthorizeQueryHandler(IRedisService redisService, IdentityQueryDbContext identityQueryDbContext)
        {
            _redisService = redisService;
            _identityQueryDbContext = identityQueryDbContext;
        }
        public async Task<bool> Handle(CheckAuthorizeQuery request, CancellationToken cancellationToken)
        {
            var authorizeFromCache =
                await _redisService.GetAsync<List<Operate>>(CacheKey.UserOperates + ":" + request.SecurityStampToken);
            if (authorizeFromCache != null!)
            {
                if (authorizeFromCache.Any(operate => operate.Title == request.Operate))
                    return true;
                return false;
            }

            var authorizeQuery =
                from user in _identityQueryDbContext.Users
                join userRole in _identityQueryDbContext.UserRoles
                    on user.Id equals userRole.UserId into userGrouping
                from userRole in userGrouping
                where user.SecurityStampToken == request.SecurityStampToken

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
                   authorize.Any(operate => operate.Title == request.Operate);
        }
    }
}
