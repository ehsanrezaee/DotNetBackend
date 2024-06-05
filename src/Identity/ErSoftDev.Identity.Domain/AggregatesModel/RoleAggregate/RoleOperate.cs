using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate
{
    public class RoleOperate : BaseEntity<long>
    {
        public long RoleId { get; private set; }
        public long OperateId { get; private set; }
        public Role Role { get; private set; }

        private RoleOperate() { }
        public RoleOperate(long roleId, long actionId)
        {
            RoleId = roleId;
            OperateId = actionId;
        }
    }
}
