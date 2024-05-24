using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class AddRoleCommand : IRequest<ApiResult>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
