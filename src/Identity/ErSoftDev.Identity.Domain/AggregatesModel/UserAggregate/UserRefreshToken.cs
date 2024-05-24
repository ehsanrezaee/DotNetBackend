using System.Text;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public class UserRefreshToken : BaseEntity<long>

    {
        public long UserId { get; private set; }
        public string Token { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsUse { get; private set; }
        public bool IsRevoke { get; private set; }
        public DateTime ExpireAt { get; private set; }

        public User User { get; private set; }



        private UserRefreshToken()
        {

        }

        public UserRefreshToken(long id, long userId, string token, bool isActive, bool isUse, bool isRevoke, DateTime expireAt)
        {
            var parameterValidation = new StringBuilder();
            if (id == 0)
                parameterValidation.Append(nameof(id) + " ");
            if (userId == 0)
                parameterValidation.Append(nameof(userId));
            if (parameterValidation.Length > 0)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.ParametersAreNotValid,
                    parameterValidation.ToString());

            Id = id;
            UserId = userId;
            Token = token;
            IsActive = isActive;
            IsUse = isUse;
            IsRevoke = isRevoke;
            ExpireAt = expireAt;
            CreatorUserId = userId;
        }

        public void DeletePreviousRefreshToken()
        {
            IsActive = false;
            IsDeleted = true;
            DeletedAt = DateTime.Now;
        }

        public void UseRefreshToken()
        {
            IsUse = true;
        }

        public void RevokeRefreshToken()
        {
            IsRevoke = true;
        }
    }
}
