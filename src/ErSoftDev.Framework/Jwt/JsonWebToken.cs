using System.Security.Claims;

namespace ErSoftDev.Framework.Jwt
{
    public class TokenRequest
    {
        public string Audience { get; set; }
        public List<Claim> Subject { get; set; }

    }
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
    }

}
