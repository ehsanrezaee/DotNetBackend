using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.Grpc;
using ErSoftDev.Identity.Application.Queries;
using ErSoftDev.Identity.EndPoint.Grpc.Protos;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.EndPoint.Grpc.Services
{
    public class AccountService : AccountGrpcService.AccountGrpcServiceBase, IGrpcService
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public AccountService(IMediator mediator, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _mediator = mediator;
            _stringLocalizer = stringLocalizer;
        }

        public override async Task<IsSecurityStampTokenResponseGrpc> IsSecurityStampTokenValid(
            IsSecurityStampTokenRequestGrpc request, ServerCallContext context)
        {
            var isSecurityStampTokenValid = await _mediator.Send(new IsSecurityStampTokenValidQuery()
            {
                SecurityStampToken = request.SecurityStampToken
            }, context.CancellationToken);


            if (isSecurityStampTokenValid)
                return new IsSecurityStampTokenResponseGrpc() { Status = (int)ApiResultStatusCode.Success };

            return new IsSecurityStampTokenResponseGrpc() { Status = (int)ApiResultStatusCode.Failed };
        }
    }
}
