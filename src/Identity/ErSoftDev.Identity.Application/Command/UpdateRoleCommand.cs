using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class UpdateRoleCommand : IRequest<ApiResult>

    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
