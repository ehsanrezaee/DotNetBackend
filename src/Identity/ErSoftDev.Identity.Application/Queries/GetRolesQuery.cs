using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Application.Dtos;
using MediatR;

namespace ErSoftDev.Identity.Application.Queries
{
    public class GetRolesQuery : PagingRequest, IRequest<ApiResult<PagedResult<RoleDto>>>
    {
        public long? Id { get; set; }
        public string? Title { get; set; }
    }
}
