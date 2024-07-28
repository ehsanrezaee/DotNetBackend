using System.Security.Claims;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Jwt;
using ErSoftDev.Framework.Redis;
using ErSoftDev.Identity.Domain;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using ErSoftDev.Identity.Domain.SeedWorks;
using IdGen;
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
        private readonly IIdGenerator<long> _idGenerator;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;
        private readonly IRedisService _redisService;
        private readonly IStringLocalizer<IdentityTranslate> _identityStringLocalizer;

        public LoginCommandHandler(IJwtService jwtService, IUserRepository userRepository,
            IOptions<AppSetting> appSetting, IIdGenerator<long> idGenerator,
            IStringLocalizer<SharedTranslate> stringLocalizer, IRedisService redisService, IStringLocalizer<IdentityTranslate> identityStringLocalizer)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _appSetting = appSetting;
            _idGenerator = idGenerator;
            _stringLocalizer = stringLocalizer;
            _redisService = redisService;
            _identityStringLocalizer = identityStringLocalizer;
        }

        public async Task<ApiResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginFromCache = await _redisService.GetAsync<LoginResponse?>(CacheKey.Login + ":" + request.Username +
                                                                              ":" + SecurityHelper.GetMd5(
                                                                                  request.Password));
            if (loginFromCache != null)
                return new ApiResult<LoginResponse>(_stringLocalizer, ApiResultStatusCode.Success, loginFromCache);

            var user = await _userRepository.GetUserByUsername(request.Username, cancellationToken);
            if (user is null)
                throw new AppException(_identityStringLocalizer, IdentityResultStatusCode.UserNotFound);
            if (!user.IsActive)
                throw new AppException(_identityStringLocalizer, IdentityResultStatusCode.UserIsNotActive);
            if (SecurityHelper.GetMd5(request.Password, user.SaltPassword).EncrypedData !=
                user.Password)
                throw new AppException(_identityStringLocalizer,
                    IdentityResultStatusCode.UsernameOrPasswordIsNotCorrect);

            var securityStampToken = Guid.NewGuid().ToString();
            var refreshTokenExpiry = DateTime.Now.AddSeconds(_appSetting.Value.Jwt.RefreshTokenExpirySecond);
            var refreshToken = user.LoginOperation(securityStampToken,
                refreshTokenExpiry, request.DeviceName, request.DeviceUniqueId,
                request.FcmToken, request.Browser, _idGenerator.CreateId(), _idGenerator.CreateId());

            var token = await _jwtService.Generate(new TokenRequest()
            {
                Subject = SetTokenClaim(securityStampToken, user.Id)
            });
            if (token.Token is null)
                throw new AppException(ApiResultStatusCode.LogicError);

            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var response = new LoginResponse()
            {
                Token = token.Token,
                TokenExpiry = token.TokenExpiry,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = refreshTokenExpiry,
            };

            await _redisService.AddOrUpdateAsync(
                CacheKey.Login + ":" + request.Username + ":" + SecurityHelper.GetMd5(request.Password), response,
                TimeSpan.FromMinutes((token.TokenExpiry - DateTime.Now).TotalMinutes - 1));

            return new ApiResult<LoginResponse>(_identityStringLocalizer, ApiResultStatusCode.Success, response);
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
