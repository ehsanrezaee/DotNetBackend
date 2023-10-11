using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ErSoftDev.Framework.Swagger
{
    public class UnauthorizedResponsesOperationFilter : IOperationFilter
    {
        private readonly bool _includeUnauthorizedAndForbiddenResponses;
        private readonly string _schemeName;

        public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
        {
            this._includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
            this._schemeName = schemeName;
        }
        // Work on all actions that find by swagger
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get all attribute of action
            var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            // Action has allow anonymous attribute 
            var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter);
            if (hasAnonymous) return;
            // Don not have authorize attribute , do not do anything
            var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter);
            if (!hasAuthorize) return;

            // Add unAuthorize error for any action than has not access
            if (_includeUnauthorizedAndForbiddenResponses)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            }

            operation.Security = new List<OpenApiSecurityRequirement>()
            {
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, Id = _schemeName
                            },
                            Name = _schemeName,
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    }
                }
            };


        }
    }
}
