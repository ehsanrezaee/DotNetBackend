using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public class UserRole : BaseEntity<long>
    {
        public long UserId { get; private set; }
        public long RoleId { get; private set; }

        public User User { get; set; }
        public Role Role { get; set; }

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
