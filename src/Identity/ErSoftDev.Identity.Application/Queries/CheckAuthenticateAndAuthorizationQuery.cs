using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class CheckAuthenticateAndAuthorizationQuery : IRequest<bool>
    {
        public string SecurityStampToken { get; set; }
        public string Operate { get; set; }

        public CheckAuthenticateAndAuthorizationQuery(string securityStampToken, string operate)
        {
            SecurityStampToken = securityStampToken;
            Operate = operate;
        }
    }
}
