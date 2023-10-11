using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SharedTranslate = ErSoftDev.DomainSeedWork.SharedTranslate;

namespace ErSoftDev.DomainSeedWork
{
    public class ApiResult
    {
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;
        [JsonProperty]
        private ApiResultStatusCode Status { get; set; }
        [JsonProperty]
        private string? Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private ApiResultErrorCode? ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string? ErrorDescription { get; set; }

        public ApiResult(IStringLocalizer<SharedTranslate> stringLocalizer,
            ApiResultStatusCode status, ApiResultErrorCode? errorCode = null,
            string? errorDescription = null)
        {
            _stringLocalizer = stringLocalizer;
            Status = status;
            Description = _stringLocalizer[status.ToString()];
            ErrorCode = errorCode;
            ErrorDescription = string.IsNullOrWhiteSpace(errorDescription) && errorCode.HasValue
                ? _stringLocalizer[errorCode.ToString() ?? string.Empty]
                : _stringLocalizer[errorCode.ToString() ?? string.Empty] == ""
                    ? null
                    : _stringLocalizer[errorCode.ToString() ?? string.Empty] + " " + errorDescription;
        }
    }

    public class ApiResult<TData> : ApiResult
        where TData : class
    {
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TData? Data { get; set; }

        public ApiResult(IStringLocalizer<SharedTranslate> stringLocalizer,
            ApiResultStatusCode status, TData? data = null, ApiResultErrorCode? errorCode = null,
            string? errorDescription = null) : base(stringLocalizer, status, errorCode,
            errorDescription)
        {
            _stringLocalizer = stringLocalizer;
            Data = data;
        }
    }

}
