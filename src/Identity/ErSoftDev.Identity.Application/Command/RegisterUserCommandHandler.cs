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
            var user = await _userRepository.Get(
                u => u.Username.ToLower() == request.Username.ToLower() && u.IsDeleted == false,
                cancellationToken);
            if (user != null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.AlreadyExists);

            var md5Password = SecurityHelper.GetMd5(request.Password);

            var newUser = new User(_idGenerator.CreateId(), request.Firstname,
                request.Lastname, request.Username,
                md5Password.EncrypedData, SecurityHelper.GetMd5(request.CheckPassword, md5Password.Salt).EncrypedData,
                md5Password.Salt, true);
            await _userRepository.Add(newUser, cancellationToken);
            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
