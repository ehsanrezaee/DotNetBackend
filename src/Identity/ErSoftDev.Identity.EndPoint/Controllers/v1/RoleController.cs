using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Application.Command;
using ErSoftDev.Identity.Application.Dtos;
using ErSoftDev.Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ErSoftDev.Identity.EndPoint.Controllers.v1
{
    [ApiVersion("1.0")]
    public class RoleController : IdentityBaseController
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<ApiResult> AddRole(AddRoleCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpGet("[action]")]
        public async Task<ApiResult<PagedResult<RoleDto>>> GetRoles(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpPut("[action]")]
        public async Task<ApiResult> UpdateRole(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }

        [HttpDelete("[action]")]
        public async Task<ApiResult> DeleteRole(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(request, cancellationToken);
        }
    }
}
