using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public class UserRole : BaseEntity<long>, ISoftDelete
    {
        public long UserId { get; private set; }
        public long RoleId { get; private set; }
        public bool IsDeleted { get; set; }
        public long? DeleterUserId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public User User { get; private set; }

        private UserRole()
        {

        }

        public UserRole(long userId, long roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }

}
