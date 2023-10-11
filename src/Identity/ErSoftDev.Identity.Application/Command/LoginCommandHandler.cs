using System.Security.Claims;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.IdGenerate;
using ErSoftDev.Framework.Jwt;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace ErSoftDev.Identity.Application.Command
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResult<LoginResponse>>
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IOptions<AppSetting> _appSetting;
        private readonly IIdGenerator _idGenerator;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public LoginCommandHandler(IJwtService jwtService, IUserRepository userRepository,
            IOptions<AppSetting> appSetting, IIdGenerator idGenerator, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _appSetting = appSetting;
            _idGenerator = idGenerator;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAndPassword(request.Username,
                SecurityHelper.GetMd5(request.Password), cancellationToken);
            if (user is null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.UsernameOrPasswordIsNotCorrect);

            var securityStampToken = Guid.NewGuid().ToString();
            var refreshTokenExpiry = DateTime.Now.AddSeconds(_appSetting.Value.Jwt.RefreshTokenExpirySecond);
            var refreshToken = user.UpdateSecurityStampTokenAndGetRefreshToken(securityStampToken,
                refreshTokenExpiry, request.DeviceName, request.DeviceUniqueId,
                request.FcmToken, request.Browser, _idGenerator.CreateId(), _idGenerator.CreateId());

            var token = await _jwtService.Generate(new TokenRequest()
            {
                Subject = SetTokenClaim(securityStampToken, user.Id)
            });
            if (token.Token is null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.LogicError);

            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var response = new LoginResponse()
            {
                Token = token.Token,
                TokenExpiry = token.TokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry,
            };
            return new ApiResult<LoginResponse>(_stringLocalizer, ApiResultStatusCode.Success, response);
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
