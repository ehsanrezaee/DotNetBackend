using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ErSoftDev.DomainSeedWork
{
    public class ApiResult
    {
        private readonly IStringLocalizer _stringLocalizer;
        [JsonProperty]
        private int Status { get; set; }
        [JsonProperty]
        private string? Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string? ErrorDescription { get; set; }

        public ApiResult(IStringLocalizer stringLocalizer,
            ApiResultStatusCode status, ApiResultErrorCode? errorCode = null,
            string? errorDescription = null)
        {
            _stringLocalizer = stringLocalizer;
            Status = status.Id;
            Description = _stringLocalizer == null || _stringLocalizer[status.ToString()].ResourceNotFound
                ? ResourceHelper.GetValue(status.ToString())
                : _stringLocalizer[status.ToString()];
            ErrorCode = errorCode?.Id;
            ErrorDescription = string.IsNullOrWhiteSpace(errorDescription) && errorCode?.Id != null
                ? _stringLocalizer == null || stringLocalizer[errorCode.ToString()].ResourceNotFound
                    ? ResourceHelper.GetValue(errorCode.ToString())
                    : _stringLocalizer[errorCode.ToString()]
                : _stringLocalizer[errorCode?.ToString() ?? string.Empty] == ""
                    ? null
                    : _stringLocalizer[errorCode?.ToString() ?? string.Empty] + " " + errorDescription;
        }
    }

    public class ApiResult<TData> : ApiResult
        where TData : class
    {
        private readonly IStringLocalizer _stringLocalizer;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TData? Data { get; set; }

        public ApiResult(IStringLocalizer stringLocalizer,
            ApiResultStatusCode status, TData? data = null, ApiResultErrorCode? errorCode = null,
            string? errorDescription = null) : base(stringLocalizer, status, errorCode,
            errorDescription)
        {
            _stringLocalizer = stringLocalizer;
            Data = data;
        }
    }

    public static class ResourceHelper
    {
        public static string GetValue(string key) =>
            key switch
            {
                "Success" => Resources.SharedTranslate.Success,
                "Failed" => Resources.SharedTranslate.Failed,
                "Unknown" => Resources.SharedTranslate.Unknown,
                "NotFound" => Resources.SharedTranslate.NotFound,
                "AlreadyExists" => Resources.SharedTranslate.AlreadyExists,
                "AuthorizationFailed" => Resources.SharedTranslate.AuthorizationFailed,
                "BadRequest" => Resources.SharedTranslate.BadRequest,
                "DbError" => Resources.SharedTranslate.DbError,
                "LogicError" => Resources.SharedTranslate.LogicError,
                "ParametersAreNotValid" => Resources.SharedTranslate.ParametersAreNotValid,
                "TokenIsNotValid" => Resources.SharedTranslate.TokenIsNotValid,
                "TokenIsExpired" => Resources.SharedTranslate.TokenIsExpired,
                "TokenHasNotClaim" => Resources.SharedTranslate.TokenHasNotClaim,
                "TokenIsNotSafeWithSecurityStamp" => Resources.SharedTranslate.TokenIsNotSafeWithSecurityStamp,
                _ => ""
            };
    }
}
