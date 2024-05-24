using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace ErSoftDev.DomainSeedWork
{
    public class ApiResult
    {
        private readonly IStringLocalizer<SharedTranslate> _stringLocalizer;
        [JsonProperty]
        private int Status { get; set; }
        [JsonProperty]
        private string? Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private int? ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        private string? ErrorDescription { get; set; }

        public ApiResult(IStringLocalizer<SharedTranslate> stringLocalizer,
            ApiResultStatusCode status, ApiResultErrorCode? errorCode = null,
            string? errorDescription = null)
        {
            _stringLocalizer = stringLocalizer;
            Status = status.Id;
            Description = _stringLocalizer[status.ToString()];
            ErrorCode = errorCode?.Id;
            ErrorDescription = string.IsNullOrWhiteSpace(errorDescription) && errorCode?.Id != null
                ? _stringLocalizer[errorCode.ToString()]
                : _stringLocalizer[errorCode?.ToString() ?? string.Empty] == ""
                    ? null
                    : _stringLocalizer[errorCode?.ToString() ?? string.Empty] + " " + errorDescription;
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
