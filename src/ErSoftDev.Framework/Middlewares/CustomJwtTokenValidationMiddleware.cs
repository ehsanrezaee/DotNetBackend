using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using ErSoftDev.DomainSeedWork;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ErSoftDev.Framework.Middlewares
{
    public class CustomJwtTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public CustomJwtTokenValidationMiddleware(RequestDelegate next, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _next = next;
            _stringLocalizer = stringLocalizer;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var executingEndpoint = httpContext.GetEndpoint();
            if (executingEndpoint != null)
            {
                var attributes = executingEndpoint!.Metadata.OfType<AllowAnonymousAttribute>().ToList();
                if (!attributes.Any())
                {
                    var token = httpContext.Request.Headers["Authorization"].ToString();
                    if (string.IsNullOrWhiteSpace(token) || !token.Contains(JwtBearerDefaults.AuthenticationScheme + " "))
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        httpContext.Response.ContentType = "Application/json";
                        await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                            new ApiResult(_stringLocalizer, ApiResultStatusCode.Failed,
                        ApiResultErrorCode.TokenIsNotValid, null),
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

                        return;
                    }
                }
            }
            await _next(httpContext);
        }
    }
}

