using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResult>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public UpdateRoleCommandHandler(IRoleRepository roleRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _roleRepository = roleRepository;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.Get(role => role.Id == request.Id, cancellationToken);
            if (role == null)
                throw new AppException(ApiResultStatusCode.NotFound);

            role.Update(request.Title, request.Description, request.IsActive);
            await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
