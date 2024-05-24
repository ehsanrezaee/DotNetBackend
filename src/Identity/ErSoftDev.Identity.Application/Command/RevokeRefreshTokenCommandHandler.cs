using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, ApiResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public RevokeRefreshTokenCommandHandler(IUserRepository userRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _userRepository = userRepository;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByRefreshToken(request.RefreshToken, cancellationToken);
            if (user == null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);

            user.RevokeRefreshToken(request.RefreshToken);

            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
