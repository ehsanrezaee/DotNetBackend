using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace ErSoftDev.ApiGateway.Extensions
{
    public static class ServiceCollectionExtension
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static HttpContext Current => _httpContextAccessor.HttpContext;
        public static void AddCustomApiGatewayJwtAuthentication(this IServiceCollection services, Jwt jwt)
        {
            var firstUse = true;
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
                    //RequireSignedTokens = true,
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
                        var stringLocalizer = context.HttpContext.RequestServices
                            .GetRequiredService<IStringLocalizer<SharedTranslate>>();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.HttpContext.Response.ContentType = "application/json";

                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(stringLocalizer,
                                ApiResultStatusCode.Failed, ApiResultErrorCode.TokenIsExpired)));
                        else if (context.Exception.GetType() == typeof(RpcException))
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(stringLocalizer,
                                ApiResultStatusCode.Failed, ApiGatewayResultErrorCode.IdentityServerIsNotAvailable)));
                        else
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(stringLocalizer,
                                ApiResultStatusCode.Failed, ApiResultErrorCode.TokenIsNotValid)));
                    },
                    OnTokenValidated = async context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims.Any() != true)
                        {
                            var stringLocalizer = context.HttpContext.RequestServices
                                .GetRequiredService<IStringLocalizer<SharedTranslate>>();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.HttpContext.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(stringLocalizer,
                                ApiResultStatusCode.Failed, ApiResultErrorCode.TokenHasNotClaim)));

                            return;
                        }

                        var securityStamp =
                            claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (securityStamp == null)
                        {
                            var stringLocalizer = context.HttpContext.RequestServices
                                .GetRequiredService<IStringLocalizer<SharedTranslate>>();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.HttpContext.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(stringLocalizer,
                                ApiResultStatusCode.Failed, ApiResultErrorCode.TokenIsExpired)));

                            return;
                        }

                        var accountService = context.HttpContext.RequestServices
                            .GetRequiredService<IAccountService>();

                        var actionPrefix = context.HttpContext.Request.Path.ToString().Split('/');
                        var actionName = actionPrefix[1] + "/" + actionPrefix[3] + "/" + actionPrefix[4] + "/" +
                                         actionPrefix[5] + "/";

                        var isAuthenticateAndAuthorize =
                            await accountService.CheckAuthenticateAndAuthorization(securityStamp, actionName);
                        if (isAuthenticateAndAuthorize.Status != ApiResultStatusCode.Success.Id)
                        {
                            var stringLocalizer = context.HttpContext.RequestServices
                                .GetRequiredService<IStringLocalizer<SharedTranslate>>();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.HttpContext.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(stringLocalizer,
                                ApiResultStatusCode.Failed, ApiGatewayResultErrorCode.AuthorizationFailed)));

                            return;
                        }

                        context.HttpContext.Request.Headers.Append("ClientIp",
                            context.HttpContext?.Connection?.RemoteIpAddress?.ToString());
                    }
                };

            });
        }
    }
}
