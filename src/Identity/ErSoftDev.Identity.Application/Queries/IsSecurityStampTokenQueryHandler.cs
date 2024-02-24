using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class IsSecurityStampTokenQueryHandler : IRequestHandler<IsSecurityStampTokenValidQuery, bool>
    {
        private readonly IUserRepository _userRepository;

        public IsSecurityStampTokenQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> Handle(IsSecurityStampTokenValidQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserBySecurityStampToken(request.SecurityStampToken, cancellationToken);
            if (user is not null)
                return true;

            return false;
        }
    }
}
