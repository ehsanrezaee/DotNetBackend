namespace ErSoftDev.DomainSeedWork
{
    public enum ApiResultStatusCode
    {
        Success = 0,
        Failed = 1,
        Unknown = 2
    }

    public enum ApiResultErrorCode
    {
        NotFound = 1,
        LogicError = 2,
        AlreadyExists = 3,
        ParametersAreNotValid = 4,
        PasswordsAreNotEqual = 5,
        AllFieldsOfAddressMustBeFillOrNonOfFields = 6,
        UsernameOrPasswordIsNotCorrect = 7,
        OneOfTheBrowserOrDeviceNameMustBeFill = 8,
        RefreshTokenIsDeActive = 9,
        RefreshTokenIsUsed = 10,
        RefreshTokenIsRevoked = 11,
        RefreshTokenIsExpire = 12,
        TokenIsExpired = 13,
        TokenHasNotClaim = 14,
        TokenIsNotSafeWithSecurityStamp = 15,
        BadRequest = 16,
        DbError = 17,
        RequestHasNotToken = 18,
        TokenIsNotValid = 19,
        SecurityStampTokenIsNotValid = 20,
        ProviderNotAvailable = 21,
        UnhandledExceptionHappened = 22,
        ProcessingIsInProgress = 23,
        IdentityServerIsNotAvailable=24
    }
}