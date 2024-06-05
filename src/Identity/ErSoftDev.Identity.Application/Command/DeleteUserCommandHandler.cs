using System.Net;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public DeleteUserCommandHandler(IUserRepository userRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _userRepository = userRepository;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(u => u.Id == request.UserId, cancellationToken);
            if (user is null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);

            _userRepository.Delete(user);

            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
