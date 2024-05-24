using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Application.Dtos;
using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class GetOperatesQuery : PagingRequest, IRequest<ApiResult<PagedResult<OperateDto>>>
    {
        public string? Title { get; set; }
    }
}
