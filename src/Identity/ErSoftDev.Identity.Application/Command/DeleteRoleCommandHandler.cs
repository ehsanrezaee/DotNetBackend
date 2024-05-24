using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.RoleAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResult>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public DeleteRoleCommandHandler(IRoleRepository roleRepository,
            IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _roleRepository = roleRepository;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.Get(role => role.Id == request.Id, cancellationToken);
            if (role == null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);

            role.Delete();

            await _roleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}