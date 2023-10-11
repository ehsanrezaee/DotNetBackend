using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public class Address : ValueObject

    {
        public string AddressLine { get; private set; }
        public string Plaque { get; private set; }
        public string Unit { get; private set; }
        public string PostalCode { get; private set; }

        private Address()
        {

        }

        public Address(string addressLine, string plaque, string unit, string postalCode)
        {
            AddressLine = addressLine;
            Plaque = plaque;
            Unit = unit;
            PostalCode = postalCode;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return AddressLine;
            yield return Plaque;
            yield return Unit;
            yield return PostalCode;
        }
    }

}