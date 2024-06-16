using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using System.Net;
using ErSoftDev.ApiGateway.Infrastructure.ServiceProviderConfiguration.Identity;
using ErSoftDev.ApiGateway.SeedWorks;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;

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
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenIsExpired, null, HttpStatusCode.Unauthorized);

                        if (context.Exception.GetType() == typeof(RpcException))
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.IdentityServerIsNotAvailable, null, HttpStatusCode.Unauthorized);

                        if (context.Exception.GetType() != typeof(AppException))
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenIsNotValid, null, HttpStatusCode.Unauthorized);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims.Any() != true)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenHasNotClaim, null, HttpStatusCode.Unauthorized);

                        var securityStamp =
                            claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (securityStamp == null)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenIsNotSafeWithSecurityStamp, null, HttpStatusCode.Unauthorized);

                        var accountService = context.HttpContext.RequestServices
                            .GetRequiredService<IAccountService>();
                        var actionPrefix = context.HttpContext.Request.Path.ToString().Split('/');
                        var actionName = actionPrefix[1] + "/" + actionPrefix[3] + "/" + actionPrefix[4] + "/" +
                                         actionPrefix[5] + "/";

                        var isAuthenticateAndAuthorize =
                            await accountService.CheckAuthenticateAndAuthorization(securityStamp, actionName);
                        if (isAuthenticateAndAuthorize.Status != ApiResultStatusCode.Success.Id)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiGatewayResultErrorCode.SecurityStampTokenIsNotValid, null, HttpStatusCode.Unauthorized);

                        context.HttpContext.Request.Headers.Append("ClientIp",
                            context.HttpContext?.Connection?.RemoteIpAddress?.ToString());
                    }
                };

            });
        }
    }
}
