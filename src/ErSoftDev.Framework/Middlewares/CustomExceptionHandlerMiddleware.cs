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
                httpContext.Request.Headers.TryGetValue("Culture", out var culture);
                if (string.IsNullOrWhiteSpace(culture.ToString()))
                    culture = "fa-IR";
                var dateInfo = CultureInfo.CreateSpecificCulture("en-us").DateTimeFormat;
                var numberInfo = CultureInfo.CreateSpecificCulture("en-us").NumberFormat;
                var currentCulture = new CultureInfo(culture) { DateTimeFormat = dateInfo, NumberFormat = numberInfo };

                Thread.CurrentThread.CurrentUICulture = currentCulture;
                Thread.CurrentThread.CurrentCulture = currentCulture;

                await _next(httpContext);

                #region TestMultiLanguage Middleware

                //var originBody = httpContext.Response.Body;
                //try
                //{
                //    var memStream = new MemoryStream();
                //    httpContext.Response.Body = memStream;

                //    await _next(httpContext).ConfigureAwait(false);

                //    memStream.Position = 0;
                //    var responseBody = new StreamReader(memStream).ReadToEnd();


                //    if (responseBody.HasValue())
                //    {
                //        var f = JsonSerializer.Deserialize<SimpleApiResult>(responseBody);
                //        httpContext.Response.ContentType = "Application/json";
                //        //await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(f.Status,_stringLocalizer[f.Status.ToString()])));
                //        responseBody = JsonConvert.SerializeObject(new ApiResult(f.Status, _stringLocalizer[f.Status.ToString()]));
                //    }

                //    //Custom logic to modify response
                //    //                    responseBody = JsonConvert.SerializeObject(new ApiResult(f.Status, _stringLocalizer[f.Status.ToString()], f.ErrorCode, _stringLocalizer[f.ErrorCode.ToString()]));

                //    var memoryStreamModified = new MemoryStream();
                //    var sw = new StreamWriter(memoryStreamModified);
                //    sw.Write(responseBody);
                //    sw.Flush();
                //    memoryStreamModified.Position = 0;

                //    await memoryStreamModified.CopyToAsync(originBody).ConfigureAwait(false);
                //}
                //finally
                //{
                //    httpContext.Response.Body = originBody;
                //}

                #endregion

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
