using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.SeedWorks
{
    public class IdentityResultStatusCode : ApiResultStatusCode
    {
        public static IdentityResultStatusCode UsernameOrPasswordIsNotCorrect =>
            new(100, nameof(UsernameOrPasswordIsNotCorrect));
        public static IdentityResultStatusCode PasswordsAreNotEqual => new(101, nameof(PasswordsAreNotEqual));
        public static IdentityResultStatusCode RefreshTokenIsDeActive => new(102, nameof(RefreshTokenIsDeActive));
        public static IdentityResultStatusCode RefreshTokenIsUsed => new(103, nameof(RefreshTokenIsUsed));
        public static IdentityResultStatusCode RefreshTokenIsRevoked => new(104, nameof(RefreshTokenIsRevoked));
        public static IdentityResultStatusCode RefreshTokenIsExpire => new(105, nameof(RefreshTokenIsExpire));
        public static IdentityResultStatusCode AllFieldsOfAddressMustBeFillOrNonOfFields => new(106, nameof(AllFieldsOfAddressMustBeFillOrNonOfFields));
        public static IdentityResultStatusCode OneOfTheBrowserOrDeviceNameMustBeFill => new(107, nameof(OneOfTheBrowserOrDeviceNameMustBeFill));
        public static IdentityResultStatusCode UserIsNotLogin => new(108, nameof(UserIsNotLogin));
        public static IdentityResultStatusCode UserNotFound => new(109, nameof(UserNotFound));
        public static IdentityResultStatusCode UserIsNotActive => new(109, nameof(UserIsNotActive));
        public static IdentityResultStatusCode Test => new(110, nameof(Test));

        protected IdentityResultStatusCode(int id, string name) : base(id, name)
        {
        }
    }
}