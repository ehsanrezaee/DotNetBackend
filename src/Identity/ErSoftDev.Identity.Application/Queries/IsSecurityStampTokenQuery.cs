using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class IsSecurityStampTokenValidQuery : IRequest<bool>
    {
        public IsSecurityStampTokenValidQuery(string securityStampToken)
        {
            SecurityStampToken = securityStampToken;
        }
        public string SecurityStampToken { get; set; }
        public string ActionName { get; set; }
    }
}
