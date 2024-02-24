using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.Grpc;
using ErSoftDev.Identity.Application.Queries;
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
                return new IsSecurityStampTokenResponseGrpc() { Status = (int)ApiResultStatusCode.Success };

            return new IsSecurityStampTokenResponseGrpc() { Status = (int)ApiResultStatusCode.Failed };
        }
    }
}
