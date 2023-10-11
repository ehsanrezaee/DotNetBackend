using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace ErSoftDev.Framework.Swagger
{
    public class AddHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Culture",
                Description = "This is optional parameter In header",
                Required = false,
                In = ParameterLocation.Header,
                Example = new OpenApiString("fa-IR"),

            });
        }
    }
}
