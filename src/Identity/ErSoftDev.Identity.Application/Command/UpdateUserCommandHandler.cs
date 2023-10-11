﻿using ErSoftDev.DomainSeedWork;
using ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.Extensions.Localization;

namespace ErSoftDev.Identity.Application.Command
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        public UpdateUserCommandHandler(IUserRepository userRepository, IStringLocalizer<SharedTranslate> stringLocalizer)
        {
            _userRepository = userRepository;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(request.Id, cancellationToken);
            if (user == null)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.NotFound);

            user.Update(request.Firstname, request.Lastname, request.CellPhone, request.Email, request.Address);

            await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return new ApiResult(_stringLocalizer, ApiResultStatusCode.Success);
        }
    }
}
