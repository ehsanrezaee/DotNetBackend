using ErSoftDev.DomainSeedWork;

namespace ErSoftDev.ApiGateway.SeedWorks
{
    public class ApiGatewayResultStatusCode : ApiResultStatusCode
    {
        public static ApiGatewayResultStatusCode IdentityServerIsNotAvailable => new(201, nameof(IdentityServerIsNotAvailable));
        public static ApiGatewayResultStatusCode AuthorizationFailed => new(202, nameof(AuthorizationFailed));

        public ApiGatewayResultStatusCode(int id, string name) : base(id, name)
        {
        }
    }
}
