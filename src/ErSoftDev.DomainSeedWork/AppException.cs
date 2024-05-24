using System.Net;
using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.DomainSeedWork
{
    public class AppException : Exception
    {
        public ApiResultStatusCode ApiResultStatusCode { get; }
        public ApiResultErrorCode ApiResultErrorCode { get; }
        public Exception? Exception { get; }
        public HttpStatusCode? HttpStatusCode { get; set; }

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
    }


    public class AppException<TEntity> : AppException
    {
        private new TEntity Data { get; }
        public AppException(Exception? exception, ApiResultStatusCode apiResultStatusCode, TEntity entity, ApiResultErrorCode apiResultErrorCode, string? message = null)
            : base(exception, apiResultStatusCode, apiResultErrorCode, message)
        {
            Data = entity;
        }


    }
}
