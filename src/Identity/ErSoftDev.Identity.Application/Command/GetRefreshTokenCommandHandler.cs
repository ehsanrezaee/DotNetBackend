using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.IdGenerate;
using ErSoftDev.Framework.Jwt;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using ErSoftDev.Framework.BaseApp;

namespace ErSoftDev.Identity.Application.Command
{
    public class GetRefreshTokenCommandHandler : IRequestHandler<GetRefreshTokenCommand, ApiResult<RefreshTokenResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly IOptions<AppSetting> _appSetting;
        private readonly IJwtService _jwtService;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public GetRefreshTokenCommandHandler(IUserRepository userRepository, IIdGenerator idGenerator,
            IOptions<AppSetting> appSetting, IJwtService jwtService, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _userRepository = userRepository;
            _idGenerator = idGenerator;
            _appSetting = appSetting;
            _jwtService = jwtService;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResult<RefreshTokenResponse>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByRefreshToken(request.RefreshToken);
            if (user == null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);

            user.RefreshTokenValidationAndUpdateToUsed(request.RefreshToken);

            var securityStampToken = Guid.NewGuid().ToString();
            var refreshTokenExpiry = DateTime.Now.AddSeconds(_appSetting.Value.Jwt.RefreshTokenExpirySecond);
            var refreshToken = user.AddNewRefreshTokenAndUpdateSecurityStampToken(securityStampToken,
                _idGenerator.CreateId(), refreshTokenExpiry);

            var token = await _jwtService.Generate(new TokenRequest()
            {
                Subject = SetTokenClaim(securityStampToken, user.Id)
            });
            if (token.Token is null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.LogicError);

            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var response = new RefreshTokenResponse()
            {
                Token = token.Token,
                TokenExpiry = token.TokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry,
            };
            return new ApiResult<RefreshTokenResponse>(_stringLocalizer, ApiResultStatusCode.Success, response);
        }

        private static List<Claim> SetTokenClaim(string securityStampToken, long userId)
        {
            var tokenSecurity = new List<Claim>
            {
                new(new ClaimsIdentityOptions().SecurityStampClaimType,
                    securityStampToken),
                new(new ClaimsIdentityOptions().UserIdClaimType,
                    userId.ToString())
            };
            return tokenSecurity;
        }
    }
}
