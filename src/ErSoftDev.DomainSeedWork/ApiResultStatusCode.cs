namespace ErSoftDev.DomainSeedWork
{
    public class ApiResultStatusCode : Enumeration
    {
        public static ApiResultStatusCode Success => new(0, nameof(Success));
        public static ApiResultStatusCode Unknown => new(1001, nameof(Unknown));
        public static ApiResultStatusCode NotFound => new(1002, nameof(NotFound));
        public static ApiResultStatusCode LogicError => new(1003, nameof(LogicError));
        public static ApiResultStatusCode AlreadyExists => new(1004, nameof(AlreadyExists));
        public static ApiResultStatusCode ParametersAreNotValid => new(1005, nameof(ParametersAreNotValid));
        public static ApiResultStatusCode BadRequest => new(1006, nameof(BadRequest));
        public static ApiResultStatusCode DbError => new(1007, nameof(DbError));
        public static ApiResultStatusCode TokenIsExpired => new(1008, nameof(TokenIsExpired));
        public static ApiResultStatusCode TokenIsNotValid => new(1009, nameof(TokenIsNotValid));
        public static ApiResultStatusCode TokenHasNotClaim => new(1010, nameof(TokenHasNotClaim));
        public static ApiResultStatusCode TokenIsNotSafeWithSecurityStamp => new(1011, nameof(TokenIsNotSafeWithSecurityStamp));
        public static ApiResultStatusCode AnUnexpectedErrorHasOccurred => new(1012, nameof(AnUnexpectedErrorHasOccurred));
        public ApiResultStatusCode(int id, string name) : base(id, name)
        {
        }
    }

    //public class ApiResultErrorCode : Enumeration
    //{


    //    public ApiResultErrorCode(int id, string name) : base(id, name)
    //    {
    //    }
    //}
}