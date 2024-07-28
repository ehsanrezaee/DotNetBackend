using System.Text;
using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.OperateAggregate
{
    public class Operate : BaseEntity<long>, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        private Operate() { }

        public Operate(string title, string description)
        {
            var parameterValidation = new StringBuilder();
            if (string.IsNullOrWhiteSpace(title))
                parameterValidation.Append(nameof(title) + " | ");
            if (string.IsNullOrWhiteSpace(description))
                parameterValidation.Append(nameof(description) + " ");
            if (parameterValidation.Length > 0)
                throw new AppException(ApiResultStatusCode.ParametersAreNotValid,
                    parameterValidation.ToString());

            Title = title;
            Description = description;
        }
    }
}
