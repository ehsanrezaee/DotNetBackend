using System.Security.Claims;
using ErSoftDev.Framework.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ErSoftDev.ApiGateway.Jwt
{
    public static class Authentication
    {
        public static void ApiGatewayAddJwtAuthentication(this IServiceCollection services, Framework.BaseApp.Jwt jwt)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //var secretKey = Encoding.UTF8.GetBytes(jwt.SecretKey);
                var secretKey = Encoding.UTF8.GetBytes(HighSecurity.JwtSecretKey);
                //var encryptKey = Encoding.UTF8.GetBytes(jwt.EncryptKey);
                var encryptKey = Encoding.UTF8.GetBytes(HighSecurity.JwtEncryptKey);
                var issuer = jwt.Issuer;
                var audience = jwt.Audience;
                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min 
                    RequireSignedTokens = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuer = false,
                    ValidIssuer = issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptKey),
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenIsExpired);

                        throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                            ApiResultErrorCode.TokenIsNotValid);
                    },
                    OnTokenValidated = async context =>
                    {
                        var stringLocalizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedTranslate>>();
                        //var ewaysBackendCoreService = context.HttpContext.RequestServices
                        //    .GetRequiredService<IEwaysBackendCoreServiceConfig>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed, ApiResultErrorCode.TokenHasNotClaim);

                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (!securityStamp.HasValue())
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed, ApiResultErrorCode.TokenIsNotSafeWithSecurityStamp);

                        //var isThereSecurityStamp = ResourceManager.ApplicationEndPointPassword.AreValueValid(new Guid(securityStamp));
                        //if (!isThereSecurityStamp)
                        //{
                        //var siteInfo = await ewaysBackendCoreService.GetEwaysSiteBySecurityStampGrpc(
                        //    new GetEwaysSiteBySecurityStmapRequestGrpc() { TokenSecurityStamp = securityStamp },
                        //    CancellationToken.None);

                        //if (siteInfo.Status != (int)ApiResultStatusCode.Success)
                        //    throw new AppException(new Exception(), ApiResultStatusCode.Failed, ApiResultErrorCode.ErrorHappenInAuthenticateUser);

                        //if (siteInfo.Data == null)
                        //    throw new AppException(new Exception(), ApiResultStatusCode.Failed, ApiResultErrorCode.Unauthorized);
                        //}

                    }
                };

            });


        }
    }
}
