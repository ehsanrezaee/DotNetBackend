namespace ErSoftDev.DomainSeedWork
{
    public class ApiResultStatusCode : Enumeration
    {
        public static ApiResultStatusCode Success => new(0, nameof(Success));
        public static ApiResultStatusCode Failed => new(1, nameof(Failed));
        public static ApiResultStatusCode Unknown => new(2, nameof(Unknown));
        public ApiResultStatusCode(int id, string name) : base(id, name)
        {
        }
    }

    public class ApiResultErrorCode : Enumeration
    {
        public static ApiResultErrorCode NotFound => new(1, nameof(NotFound));
        public static ApiResultErrorCode LogicError => new(2, nameof(LogicError));
        public static ApiResultErrorCode AlreadyExists => new(3, nameof(AlreadyExists));
        public static ApiResultErrorCode ParametersAreNotValid => new(4, nameof(ParametersAreNotValid));
        public static ApiResultErrorCode BadRequest => new(5, nameof(BadRequest));
        public static ApiResultErrorCode DbError => new(6, nameof(DbError));
        public static ApiResultErrorCode TokenIsExpired => new(7, nameof(TokenIsExpired));
        public static ApiResultErrorCode TokenIsNotValid => new(8, nameof(TokenIsNotValid));
        public static ApiResultErrorCode TokenHasNotClaim => new(9, nameof(TokenHasNotClaim));
        public static ApiResultErrorCode TokenIsNotSafeWithSecurityStamp => new(10, nameof(TokenIsNotSafeWithSecurityStamp));

        public static ApiResultErrorCode AnUnexpectedErrorHasOccurred => new(11, nameof(AnUnexpectedErrorHasOccurred));

        public ApiResultErrorCode(int id, string name) : base(id, name)
        {
        }
    }
}