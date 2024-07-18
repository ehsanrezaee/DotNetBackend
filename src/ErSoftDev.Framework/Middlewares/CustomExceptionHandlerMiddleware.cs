using System.Globalization;
using System.Net;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace ErSoftDev.Framework.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;
        private readonly IOptions<AppSetting> _appSetting;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger,
            IStringLocalizer<SharedTranslate> stringLocalizer,
            IOptions<AppSetting> appSetting)
        {
            _next = next;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _appSetting = appSetting;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (AppException ex)
            {
                _logger.LogFatal("ExceptionHandler", new { Exception = ex });

                if (ex.HttpStatusCode is not null)
                    httpContext.Response.StatusCode = (int)ex.HttpStatusCode;
                httpContext.Response.ContentType = "Application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                    new ApiResult(_stringLocalizer, ex.ApiResultStatusCode, ex.ApiResultErrorCode, ex.Message),
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            }
            catch (Exception ex)
            {
                _logger.LogFatal("ExceptionHandler", new { Exception = ex });

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "Application/json";
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(
                    new ApiResult(_stringLocalizer, ApiResultStatusCode.Failed,
                        ApiResultErrorCode.AnUnexpectedErrorHasOccurred,
                        _appSetting.Value.ShowExceptionMessage ? " - " + ex.Message + " - " + ex.StackTrace : null),
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            }
        }
    }
}
