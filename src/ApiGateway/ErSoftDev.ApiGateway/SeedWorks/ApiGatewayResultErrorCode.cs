using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.ApiGateway.SeedWorks
{
    public class ApiGatewayResultErrorCode : ApiResultErrorCode
    {
        public static ApiGatewayResultErrorCode IdentityServerIsNotAvailable => new(201, nameof(IdentityServerIsNotAvailable));
        public static ApiGatewayResultErrorCode SecurityStampTokenIsNotValid => new(202, nameof(SecurityStampTokenIsNotValid));

        public ApiGatewayResultErrorCode(int id, string name) : base(id, name)
        {
        }
    }
}
