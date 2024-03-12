using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.SeedWorks
{
    public class IdentityResultErrorCode : ApiResultErrorCode
    {
        public static IdentityResultErrorCode UsernameOrPasswordIsNotCorrect =>
            new(100, nameof(UsernameOrPasswordIsNotCorrect));
        public static IdentityResultErrorCode PasswordsAreNotEqual => new(101, nameof(PasswordsAreNotEqual));
        public static IdentityResultErrorCode RefreshTokenIsDeActive => new(102, nameof(RefreshTokenIsDeActive));
        public static IdentityResultErrorCode RefreshTokenIsUsed => new(103, nameof(RefreshTokenIsUsed));
        public static IdentityResultErrorCode RefreshTokenIsRevoked => new(104, nameof(RefreshTokenIsRevoked));
        public static IdentityResultErrorCode RefreshTokenIsExpire => new(105, nameof(RefreshTokenIsExpire));
        public static IdentityResultErrorCode AllFieldsOfAddressMustBeFillOrNonOfFields => new(106, nameof(AllFieldsOfAddressMustBeFillOrNonOfFields));
        public static IdentityResultErrorCode OneOfTheBrowserOrDeviceNameMustBeFill => new(107, nameof(OneOfTheBrowserOrDeviceNameMustBeFill));

        protected IdentityResultErrorCode(int id, string name) : base(id, name)
        {
        }
    }
}
