using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.Grpc;
using ErSoftDev.Identity.Application.Queries;
using ErSoftDev.Identity.Domain.SeedWorks;
using ErSoftDev.Identity.EndPoint.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace ErSoftDev.Identity.EndPoint.Grpc.Services
{
    public class AccountService : AccountGrpcService.AccountGrpcServiceBase, IGrpcService
    {
        private readonly IMediator _mediator;

        public AccountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<IsSecurityStampTokenResponseGrpc> IsSecurityStampTokenValid(
            IsSecurityStampTokenRequestGrpc request, ServerCallContext context)
        {
            var isSecurityStampTokenValid = await _mediator.Send(
                new IsSecurityStampTokenValidQuery(request.SecurityStampToken)
                , context.CancellationToken);
            if (isSecurityStampTokenValid)
                return new IsSecurityStampTokenResponseGrpc() { Status = ApiResultStatusCode.Success.Id };
            return new IsSecurityStampTokenResponseGrpc() { Status = IdentityResultStatusCode.TokenIsNotValid.Id };
        }

        public override async Task<CheckAuthorizeResponseGrpc> CheckAuthorize(CheckAuthorizeRequestGrpc request,
            ServerCallContext context)
        {
            var isAuthorize = await _mediator.Send(new CheckAuthorizeQuery(request.SecurityStampToken, request.Operate),
                context.CancellationToken);
            if (isAuthorize)
                return new CheckAuthorizeResponseGrpc() { Status = ApiResultStatusCode.Success.Id };
            return new CheckAuthorizeResponseGrpc() { Status = ApiResultStatusCode.TokenIsNotValid.Id };
        }

        public override async Task<CheckAuthenticationAndAuthorizationGrpcResponse> CheckAuthenticationAndAuthorization(CheckAuthenticationAndAuthorizationGrpcRequest request,
            ServerCallContext context)
        {
            var isAuthenticateAndAuthorize =
                await _mediator.Send(
                    new CheckAuthenticateAndAuthorizationQuery(request.SecurityStampToken, request.Operate),
                    context.CancellationToken);
            if (isAuthenticateAndAuthorize)
                return new CheckAuthenticationAndAuthorizationGrpcResponse()
                { Status = ApiResultStatusCode.Success.Id };
            return new CheckAuthenticationAndAuthorizationGrpcResponse() { Status = ApiResultStatusCode.TokenIsNotValid.Id };
        }
    }
}
