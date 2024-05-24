using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, ApiResult>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public AddRoleCommandHandler(IRoleRepository roleRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _roleRepository = roleRepository;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.Get(role => role.Title == request.Title, cancellationToken);
            if (role is not null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.AlreadyExists);

            var newRole = new Role(request.Title, request.Description, request.Active);
            await _roleRepository.Add(newRole, cancellationToken);
            await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
