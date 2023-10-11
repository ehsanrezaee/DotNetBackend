using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class RevokeRefreshTokenCommand : IRequest<ApiResult>
    {
        public string RefreshToken { get; set; }
    }
}
