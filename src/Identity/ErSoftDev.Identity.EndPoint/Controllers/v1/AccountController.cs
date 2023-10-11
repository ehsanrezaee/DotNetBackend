﻿using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.Api;
using ErSoftDev.Identity.Application.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErSoftDev.Identity.EndPoint.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult> Register(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult<LoginResponse>> Login(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult<RefreshTokenResponse>> GetRefreshToken(GetRefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ApiResult> RevokeRefreshToken(RevokeRefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPut("[action]")]
        public async Task<ApiResult> Update(UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}
