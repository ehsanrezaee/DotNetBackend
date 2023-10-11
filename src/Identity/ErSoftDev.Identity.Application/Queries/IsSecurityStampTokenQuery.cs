using ErSoftDev.DomainSeedWork;
using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class IsSecurityStampTokenValidQuery : IRequest<bool>
    {
        public string SecurityStampToken { get; set; }
    }
}
