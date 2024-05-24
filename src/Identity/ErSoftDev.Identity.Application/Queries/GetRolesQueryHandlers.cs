using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Application.Dtos;
using ErSoftDev.Identity.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Queries
{
    internal class GetRolesQueryHandlers : IRequestHandler<GetRolesQuery, ApiResult<PagedResult<RoleDto>>>
    {
        private readonly IdentityQueryDbContext _identityQueryDbContext;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public GetRolesQueryHandlers(IdentityQueryDbContext identityQueryDbContext,
            IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _identityQueryDbContext = identityQueryDbContext;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResult<PagedResult<RoleDto>>> Handle(GetRolesQuery request,
            CancellationToken cancellationToken)
        {
            var roles = await _identityQueryDbContext.Roles.Where(role =>
                    role.IsDeleted == false &&
                    (request.Id == null || role.Id == request.Id) &&
                    (request.Title == null || EF.Functions.Like(role.Title, "%" + request.Title + "%")))
                .Select(role => new { role.Id, role.Title, role.Description })
                .OrderBy(request.OrderBy, request.OrderType.ToString())
                .GetPaged(request.PageNumber, request.PageSize, cancellationToken);

            return new ApiResult<PagedResult<RoleDto>>(_stringLocalizer, ApiResultStatusCode.Success,
                roles.MapTo<PagedResult<RoleDto>>());
        }
    }
}
