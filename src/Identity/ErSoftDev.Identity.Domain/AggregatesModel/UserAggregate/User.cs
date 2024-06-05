using System.Text;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using ErSoftDev.Identity.Domain.SeedWorks;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public class User : BaseEntity<long>, ISoftDelete, IAggregateRoot
    {
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Username { get; private set; }
        public string SaltPassword { get; set; }
        public string Password { get; private set; }
        public string? CellPhone { get; private set; }
        public string? Email { get; private set; }
        public string? Image { get; private set; }
        public string SecurityStampToken { get; private set; }
        public bool IsActive { get; set; }
        public Address? Address { get; private set; }
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletedAt { get; set; }

        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

        private readonly List<UserLogin> _userLogins = new();
        public IReadOnlyCollection<UserLogin> UserLogins => _userLogins;

        private readonly List<UserRefreshToken> _userRefreshTokens = new();
        public IReadOnlyCollection<UserRefreshToken> UserRefreshTokens => _userRefreshTokens;

        private User() { }

        public User(long id, string firstname, string lastname, string username, string password, string checkPassword, string saltPassword, bool isActive)
        {
            var parameterValidation = new StringBuilder();
            if (id == 0)
                parameterValidation.Append(nameof(id) + " ");
            if (string.IsNullOrWhiteSpace(firstname))
                parameterValidation.Append(nameof(firstname) + " ");
            if (string.IsNullOrWhiteSpace(lastname))
                parameterValidation.Append(nameof(lastname) + " ");
            if (string.IsNullOrWhiteSpace(username))
                parameterValidation.Append(nameof(username) + " ");
            if (string.IsNullOrWhiteSpace(password))
                parameterValidation.Append(nameof(password) + " ");
            if (string.IsNullOrWhiteSpace(checkPassword))
                parameterValidation.Append(nameof(checkPassword) + " ");
            if (string.IsNullOrWhiteSpace(saltPassword))
                parameterValidation.Append(nameof(SaltPassword) + " ");
            if (parameterValidation.Length > 0)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.ParametersAreNotValid,
                    parameterValidation.ToString());

            if (password != checkPassword)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.PasswordsAreNotEqual);

            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Username = username;
            Password = password;
            CreatorUserId = id;
            SaltPassword = saltPassword;
            IsActive = isActive;
        }

        public void Update(string? firstname, string? lastname, string? cellPhone, string? email, Address? address, bool? isActive)
        {
            if (address is not null)
            {
                var addressProperty = address.GetType().GetProperties();
                var addressPropertiesAtLeastHasOneValue = false;
                var addressPropertiesAtLeastHasOneNotValue = false;
                foreach (var property in addressProperty)
                {
                    var value = property.GetValue(address, null);
                    if (value != null)
                        addressPropertiesAtLeastHasOneValue = true;
                    else
                        addressPropertiesAtLeastHasOneNotValue = true;
                }

                if (addressPropertiesAtLeastHasOneValue && addressPropertiesAtLeastHasOneNotValue)
                    throw new AppException(ApiResultStatusCode.Failed,
                        IdentityResultErrorCode.AllFieldsOfAddressMustBeFillOrNonOfFields);
            }

            Firstname = firstname ?? Firstname;
            Lastname = lastname ?? Lastname;
            CellPhone = cellPhone ?? CellPhone;
            Email = email ?? Email;
            Address = address ?? Address;
            IsActive = isActive ?? IsActive;
        }

        public string LoginOperation(string securityStampToken,
            DateTime refreshTokenExpiry, string? deviceName, string? deviceUniqueId, string? fcmToken, string? browser,
            long refreshTokenId, long loginId)
        {
            SecurityStampToken = securityStampToken;

            var refreshToken = NewRefreshToken();

            _userRefreshTokens.RemoveAll(token => true);

            _userRefreshTokens.Add(new UserRefreshToken(refreshTokenId, Id, refreshToken, true, false, false,
                refreshTokenExpiry));

            _userLogins.Add(new UserLogin(loginId, Id, deviceName, deviceUniqueId, fcmToken, browser));

            return refreshToken;
        }

        public void RefreshTokenValidationAndUpdateToUsed(string refreshToken)
        {
            var userRefreshToken = _userRefreshTokens.FirstOrDefault(token => token.Token == refreshToken);
            if (userRefreshToken is null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);
            if (userRefreshToken.IsActive == false)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsDeActive);
            if (userRefreshToken.IsUse)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsUsed);
            if (userRefreshToken.IsRevoke)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsRevoked);
            if (userRefreshToken.ExpireAt < DateTime.Now)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsExpire);

            userRefreshToken.UseRefreshToken();
        }

        public string AddNewRefreshTokenAndUpdateSecurityStampToken(string securityStampToken, long id,
            DateTime refreshTokenExpiry)
        {
            SecurityStampToken = securityStampToken;

            _userRefreshTokens.RemoveAll(token => true);

            var refreshToken = NewRefreshToken();
            _userRefreshTokens.Add(new UserRefreshToken(id, Id, refreshToken, true, false, false,
                refreshTokenExpiry));

            return refreshToken;
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            SecurityStampToken = Guid.NewGuid().ToString();

            var userRefreshToken = _userRefreshTokens.FirstOrDefault(token => token.Token == refreshToken);
            if (userRefreshToken is null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);
            if (userRefreshToken.IsActive == false)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsDeActive);
            if (userRefreshToken.IsUse)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsUsed);
            if (userRefreshToken.IsRevoke)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsRevoked);
            if (userRefreshToken.ExpireAt < DateTime.Now)
                throw new AppException(ApiResultStatusCode.Failed, IdentityResultErrorCode.RefreshTokenIsExpire);

            userRefreshToken.RevokeRefreshToken();
        }

        private string NewRefreshToken()
        {
            return string.Concat(Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
        }
    }
}
