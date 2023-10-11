using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class GetRefreshTokenCommand : IRequest<ApiResult<RefreshTokenResponse>>
    {
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
