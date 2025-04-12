using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Infrastructure.NoSql.Models
{
    public class Instrument : BaseEntity<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
