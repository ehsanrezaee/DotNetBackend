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
                new IsSecurityStampTokenRequestGrpc()
                { SecurityStampToken = securityStampToken });
        }

        public async Task<CheckAuthorizeResponseGrpc> IsAuthorize(string securityStampToken, string operate)
        {
            return await _accountGrpcServiceClient.CheckAuthorizeAsync(new CheckAuthorizeRequestGrpc()
            { SecurityStampToken = securityStampToken, Operate = operate });
        }

        public async Task<CheckAuthenticationAndAuthorizationGrpcResponse> CheckAuthenticateAndAuthorization(string securityStampToken, string operate)
        {
            return await _accountGrpcServiceClient.CheckAuthenticationAndAuthorizationAsync(
                new CheckAuthenticationAndAuthorizationGrpcRequest()
                { SecurityStampToken = securityStampToken, Operate = operate });
        }
    }
}
