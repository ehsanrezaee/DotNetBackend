using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class UpdateUserCommand : IRequest<ApiResult>
    {
        public long Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? CellPhone { get; set; }
        public string? Email { get; set; }
        public Address? Address { get; set; }
        public bool? IsActive { get; set; }
    }
}
