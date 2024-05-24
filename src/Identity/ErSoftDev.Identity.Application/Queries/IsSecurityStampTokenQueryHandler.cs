using ErSoftDev.Identity.Application.Queries;
using ErSoftDev.Identity.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ErSoftDev.Identity.Application.Queries
{
    public class IsSecurityStampTokenQueryHandler : IRequestHandler<IsSecurityStampTokenValidQuery, bool>
    {
        private readonly IdentityQueryDbContext _identityQueryDbContext;

        public IsSecurityStampTokenQueryHandler(IdentityQueryDbContext identityQueryDbContext)
        {
            _identityQueryDbContext = identityQueryDbContext;
        }
        public async Task<bool> Handle(IsSecurityStampTokenValidQuery request, CancellationToken cancellationToken)
        {
            var userInfo =
                await _identityQueryDbContext.Users.FirstOrDefaultAsync(user =>
                    user.SecurityStampToken == request.SecurityStampToken, cancellationToken);
            if (userInfo is null)
                return false;
            return true;
        }
    }
}
