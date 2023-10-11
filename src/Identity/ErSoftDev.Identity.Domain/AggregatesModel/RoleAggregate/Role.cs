using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;

namespace ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate
{
    public class Role : BaseEntity<long>, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }


        private readonly List<UserRole> _userRoles;
        public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

        private Role()
        {

        }

        public Role(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }
}
