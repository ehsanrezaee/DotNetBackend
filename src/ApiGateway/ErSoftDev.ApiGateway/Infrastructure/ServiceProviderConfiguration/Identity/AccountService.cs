using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.Configuration;
using ErSoftDev.Identity.EndPoint.Grpc.Protos;

namespace ErSoftDev.ApiGateway.Infrastructure.ServiceProviderConfiguration.Identity
{
    public class AccountService : IAccountService, ITransientDependency
    {
        private readonly AccountGrpcService.AccountGrpcServiceClient _accountGrpcServiceClient;

        public AccountService(AccountGrpcService.AccountGrpcServiceClient accountGrpcServiceClient)
        {
            _accountGrpcServiceClient = accountGrpcServiceClient;
        }

        public async Task<IsSecurityStampTokenResponseGrpc> IsSecurityStampTokenValid(string securityStampToken)
        {
            return await _accountGrpcServiceClient.IsSecurityStampTokenValidAsync(
                new IsSecurityStampTokenRequestGrpc() { SecurityStampToken = securityStampToken });
        }
    }
}
