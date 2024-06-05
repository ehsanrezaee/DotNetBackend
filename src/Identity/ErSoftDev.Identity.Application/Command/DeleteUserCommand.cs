using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class DeleteUserCommand : IRequest<ApiResult>
    {
        public long UserId { get; set; }
    }
}
