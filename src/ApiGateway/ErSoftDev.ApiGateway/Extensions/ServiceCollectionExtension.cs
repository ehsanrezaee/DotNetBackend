using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ErSoftDev.ApiGateway.Infrastructure.ServiceProviderConfiguration.Identity;
using ErSoftDev.ApiGateway.SeedWorks;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;

namespace ErSoftDev.ApiGateway.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddCustomApiGatewayJwtAuthentication(this IServiceCollection services, Jwt jwt)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(HighSecurity.JwtSecretKey);
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

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptKey)
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
                                ApiGatewayResultErrorCode.TokenIsExpired);

                        if (context.Exception.GetType() == typeof(RpcException))
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.IdentityServerIsNotAvailable);

                        if (context.Exception.GetType() != typeof(AppException))
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.TokenIsNotValid);
                    },
                    OnTokenValidated = async context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims.Any() != true)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.TokenHasNotClaim);

                        var securityStamp =
                            claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (securityStamp == null)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.TokenIsNotSafeWithSecurityStamp);

                        var accountService = context.HttpContext.RequestServices
                            .GetRequiredService<IAccountService>();
                        var isSecurityStampTokenValid =
                            await accountService.IsSecurityStampTokenValid(securityStamp);
                        if (isSecurityStampTokenValid.Status != ApiResultStatusCode.Success.Id)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.SecurityStampTokenIsNotValid);
                    }
                };

            });
        }
    }
}
