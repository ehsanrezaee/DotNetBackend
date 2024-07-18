using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace ErSoftDev.Framework.Middlewares
{
    public class CustomStringLocalizedMiddleware

    {
        private readonly RequestDelegate _next;
        public CustomStringLocalizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            httpContext.Request.Headers.TryGetValue("Culture", out var culture);
            if (string.IsNullOrWhiteSpace(culture.ToString()))
                culture = "fa-IR";
            var dateInfo = CultureInfo.CreateSpecificCulture("en-us").DateTimeFormat;
            var numberInfo = CultureInfo.CreateSpecificCulture("en-us").NumberFormat;
            var currentCulture = new CultureInfo(culture!) { DateTimeFormat = dateInfo, NumberFormat = numberInfo };

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
            //        var f = System.Text.Json.JsonSerializer.Deserialize<ApiResult>(responseBody);
            //        httpContext.Response.ContentType = "Application/json";
            //        //await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResult(f.Status,_stringLocalizer[f.Status.ToString()])));
            //        //responseBody = JsonConvert.SerializeObject(new ApiResult(f.Status, _stringLocalizer[f.Status.ToString()]));
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
    }
}
