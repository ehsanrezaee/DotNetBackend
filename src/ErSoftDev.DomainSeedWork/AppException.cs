using System.Net;
using ErSoftDev.DomainSeedWork;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.DomainSeedWork
{
    public class AppException : Exception
    {
        public ApiResultStatusCode ApiResultStatusCode { get; }
        public ApiResultErrorCode ApiResultErrorCode { get; }
        public Exception? Exception { get; }
        public HttpStatusCode? HttpStatusCode { get; set; }
        public IStringLocalizer StringLocalizer { get; set; }

        public AppException(Exception? exception)
        {
            Exception = exception;
        }

        public AppException(Exception? exception, ApiResultStatusCode apiResultStatusCode,
            ApiResultErrorCode apiResultErrorCode, string? message = null, HttpStatusCode? httpStatusCode = null)
            : base(message ?? String.Empty)
        {
            ApiResultStatusCode = apiResultStatusCode;
            ApiResultErrorCode = apiResultErrorCode;
            Exception = exception;
            HttpStatusCode = httpStatusCode;
        }

        public AppException(ApiResultStatusCode apiResultStatusCode, ApiResultErrorCode apiResultErrorCode,
            string? message = null, HttpStatusCode? httpStatusCode = null)
            : base(message ?? string.Empty)
        {
            ApiResultStatusCode = apiResultStatusCode;
            ApiResultErrorCode = apiResultErrorCode;
            HttpStatusCode = httpStatusCode;
        }

        public AppException(IStringLocalizer stringLocalizer, ApiResultStatusCode apiResultStatusCode, ApiResultErrorCode apiResultErrorCode,
            string? message = null, HttpStatusCode? httpStatusCode = null)
            : base(message ?? string.Empty)
        {
            ApiResultStatusCode = apiResultStatusCode;
            ApiResultErrorCode = apiResultErrorCode;
            HttpStatusCode = httpStatusCode;
            StringLocalizer = stringLocalizer;
        }
    }


    public class AppException<TStringLocalizer> : AppException
    {
        public AppException(IStringLocalizer<TStringLocalizer> stringLocalizer, ApiResultStatusCode apiResultStatusCode,
            ApiResultErrorCode apiResultErrorCode, string? message = null,
            HttpStatusCode? httpStatusCode = null) : base(stringLocalizer, apiResultStatusCode, apiResultErrorCode,
            message, httpStatusCode)
        {
        }
    }
}
