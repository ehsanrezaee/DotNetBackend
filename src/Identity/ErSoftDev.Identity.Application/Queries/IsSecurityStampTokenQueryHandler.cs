using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Queries
{
    public class IsSecurityStampTokenQueryHandler : IRequestHandler<IsSecurityStampTokenValidQuery, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public IsSecurityStampTokenQueryHandler(IUserRepository userRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _userRepository = userRepository;
            _stringLocalizer = stringLocalizer;
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
