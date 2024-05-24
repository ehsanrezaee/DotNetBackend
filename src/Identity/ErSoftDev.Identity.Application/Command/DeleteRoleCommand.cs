using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class DeleteRoleCommand : IRequest<ApiResult>
    {
        public long Id { get; set; }
    }
}
