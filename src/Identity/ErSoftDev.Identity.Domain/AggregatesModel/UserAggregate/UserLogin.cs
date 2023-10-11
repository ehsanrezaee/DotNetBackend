﻿using ErSoftDev.DomainSeedWork;
using System.Text;

namespace ErSoftDev.Identity.Domain.AggregatesModel.UserAggregate
{
    public class UserLogin : BaseEntity<long>, IAggregateRoot
    {
        public long UserId { get; set; }
        public string? DeviceName { get; private set; }
        public string? DeviceUniqueId { get; private set; }
        public string? FcmToken { get; private set; }
        public string? Browser { get; private set; }

        public User User { get; set; }

        private UserLogin()
        {
        }

        public UserLogin(long id, long userId, string? deviceName, string? deviceUniqueId, string? fcmToken, string? browser)
        {
            var parameterValidation = new StringBuilder();
            if (id == 0)
                parameterValidation.Append(nameof(id) + " ");
            if (userId == 0)
                parameterValidation.Append(nameof(userId));
            if (parameterValidation.Length > 0)
                throw new AppException(ApiResultStatusCode.Failed, ApiResultErrorCode.ParametersAreNotValid,
                    parameterValidation.ToString());

            if (string.IsNullOrWhiteSpace(deviceName) && string.IsNullOrWhiteSpace(browser))
                throw new AppException(ApiResultStatusCode.Failed,
                    ApiResultErrorCode.OneOfTheBrowserOrDeviceNameMustBeFill);

            Id = id;
            UserId = userId;
            DeviceName = deviceName;
            DeviceUniqueId = deviceUniqueId;
            FcmToken = fcmToken;
            Browser = browser;
            CreatorUserId = userId;
        }
    }
}
