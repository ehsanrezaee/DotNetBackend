using ErSoftDev.Identity.EndPoint.Grpc.Protos;

namespace ErSoftDev.ApiGateway.Infrastructure.ServiceProviderConfiguration.Identity;

public interface IAccountService
{
    Task<IsSecurityStampTokenResponseGrpc> IsSecurityStampTokenValid(string securityStampToken);
    Task<CheckAuthorizeResponseGrpc> IsAuthorize(string securityStampToken, string operate);

    Task<CheckAuthenticationAndAuthorizationGrpcResponse> CheckAuthenticateAndAuthorization(string securityStampToken,
        string operate);
}