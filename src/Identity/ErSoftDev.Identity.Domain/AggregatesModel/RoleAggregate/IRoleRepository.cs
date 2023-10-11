using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate
{
    public interface IRoleRepository : IRepository<Role>, IAggregateRoot
    {

    }
}
