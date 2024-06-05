using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Application.Dtos;
using ErSoftDev.Identity.Application.Queries;
using ErSoftDev.Identity.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Queries
{
    public class GetOperatesQueryHandler : IRequestHandler<GetOperatesQuery, ApiResult<PagedResult<OperateDto>>>
    {
        private readonly IdentityQueryDbContext _identityQueryDbContext;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public GetOperatesQueryHandler(IdentityQueryDbContext identityQueryDbContext, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _identityQueryDbContext = identityQueryDbContext;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult<PagedResult<OperateDto>>> Handle(GetOperatesQuery request, CancellationToken cancellationToken)
        {
            var operates = await _identityQueryDbContext.Operates.Where(operate =>
                    (request.Title == null || EF.Functions.Like(operate.Title, "%" + request.Title + "%"))).Select(
                    operate => new
                    {
                        operate.Id,
                        operate.Title,
                        operate.Description
                    }).OrderBy(request.OrderBy, request.OrderType.ToString())
                .GetPaged(request.PageNumber, request.PageSize, cancellationToken);

            return new ApiResult<PagedResult<OperateDto>>(_stringLocalizer, ApiResultStatusCode.Success,
                operates.MapTo<PagedResult<OperateDto>>());
        }
    }
}
