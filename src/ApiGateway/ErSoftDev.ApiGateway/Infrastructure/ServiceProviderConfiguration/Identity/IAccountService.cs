using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.EndPoint.Grpc.Protos;

namespace ErSoftDev.ApiGateway.Infrastructure.ServiceProviderConfiguration.Identity;

public interface IAccountService
{
    Task<IsSecurityStampTokenResponseGrpc> IsSecurityStampTokenValid(string securityStampToken);
}