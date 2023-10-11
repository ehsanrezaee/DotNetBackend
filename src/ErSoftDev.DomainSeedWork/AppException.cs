namespace ErSoftDev.DomainSeedWork
{
    public class AppException : Exception
    {
        public ApiResultStatusCode ApiResultStatusCode { get; }
        public ApiResultErrorCode ApiResultErrorCode { get; }
        public Exception? Exception { get; }

        public AppException(Exception? exception)
        {
            Exception = exception;
        }
        public AppException(Exception? exception, ApiResultStatusCode apiResultStatusCode, ApiResultErrorCode apiResultErrorCode, string? message = null)
            : base(message ?? String.Empty)
        {
            ApiResultStatusCode = apiResultStatusCode;
            ApiResultErrorCode = apiResultErrorCode;
            Exception = exception;
        }

        public AppException(ApiResultStatusCode apiResultStatusCode, ApiResultErrorCode apiResultErrorCode, string? message = null)
            : base(message ?? string.Empty)
        {
            ApiResultStatusCode = apiResultStatusCode;
            ApiResultErrorCode = apiResultErrorCode;
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
