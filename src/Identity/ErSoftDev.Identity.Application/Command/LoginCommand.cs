using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Command
{
    public class LoginCommand : IRequest<ApiResult<LoginResponse>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? DeviceName { get; set; }
        public string? DeviceUniqueId { get; set; }
        public string? FcmToken { get; set; }
        public string? Browser { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
