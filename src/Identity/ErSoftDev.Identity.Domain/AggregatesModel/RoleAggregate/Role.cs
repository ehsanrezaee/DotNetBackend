using System.Text;
using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate
{
    public class Role : BaseEntity<long>, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<RoleOperate> _roleOperates;
        public IReadOnlyCollection<RoleOperate> RoleOperates => _roleOperates;

        private Role() { }

        public Role(long id, string title, string description, bool isActive)
        {
            var parameterValidation = new StringBuilder();
            if (string.IsNullOrWhiteSpace(title))
                parameterValidation.Append(nameof(title) + " | ");
            if (string.IsNullOrWhiteSpace(description))
                parameterValidation.Append(nameof(description) + " ");
            if (parameterValidation.Length > 0)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.ParametersAreNotValid,
                    parameterValidation.ToString());

            Id = id;
            Title = title;
            Description = description;
            IsActive = isActive;
        }

        public void Update(string? title, string? description, bool? isActive)
        {
            Title = title ?? Title;
            Description = description ?? Description;
            IsActive = isActive ?? IsActive;
        }
    }
}
