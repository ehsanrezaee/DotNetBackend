using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace ErSoftDev.DomainSeedWork
{
    public class ApiResult
    {
        [JsonProperty]
        public int Status { get; private set; }
        [JsonProperty]
        public string Description { get; private set; }

        public string? Message { get; private set; }

        public ApiResult(IStringLocalizer stringLocalizer,
            ApiResultStatusCode status, string? message = null
            )
        {
            var strLocalizer = stringLocalizer;
            Status = status.Code;
            Description = strLocalizer[status.ToString()].ResourceNotFound
                ? ResourceHelper.GetValue(status.ToString())
                : strLocalizer[status.ToString()];
            Message = message;
        }
    }

    public class ApiResult<TData> : ApiResult
        where TData : class
    {
        private readonly IStringLocalizer _stringLocalizer;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TData? Data { get; set; }

        public ApiResult(IStringLocalizer stringLocalizer,
            ApiResultStatusCode status, TData? data = null, string? message = null) : base(stringLocalizer, status, message)
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
