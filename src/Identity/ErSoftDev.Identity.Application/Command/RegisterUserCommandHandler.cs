using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using IdGen;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResult>
    {
        private readonly IIdGenerator<long> _idGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public RegisterUserCommandHandler(IIdGenerator<long> idGenerator, IUserRepository userRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _idGenerator = idGenerator;
            _userRepository = userRepository;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userIsExist = await _userRepository.GetList(user =>
                user.Username.ToLower() == request.Username.ToLower(), cancellationToken);
            if (userIsExist.Count > 0)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.AlreadyExists);

            var user = new User(_idGenerator.CreateId(), request.Firstname, request.Lastname, request.Username,
                SecurityHelper.GetMd5(request.Password), SecurityHelper.GetMd5(request.CheckPassword));
            await _userRepository.Add(user, cancellationToken);
            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
