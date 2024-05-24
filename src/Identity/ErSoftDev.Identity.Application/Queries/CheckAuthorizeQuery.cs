using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class CheckAuthorizeQuery : IRequest<bool>
    {
        public string SecurityStampToken { get; set; }
        public string Operate { get; set; }

        public CheckAuthorizeQuery(string securityStampToken, string operate)
        {
            SecurityStampToken = securityStampToken;
            Operate = operate;
        }
    }
}
