using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;


namespace ErSoftDev.Framework.Middlewares
{
    public static class CustomSwaggerUiMiddleware
    {
        public static void UseCustomSwaggerUi(this IApplicationBuilder applicationBuilder, BaseApp.Swagger? swagger)
        {
            if (swagger == null)
                return;

            applicationBuilder.UseSwagger();

            var versionCount = swagger.VersionCount;
            decimal versionStart = (decimal)0.9;
            const decimal versionStep = (decimal)0.1;


            applicationBuilder.UseSwaggerUI(options =>
            {
                for (var i = 0; i < versionCount; i++)
                {
                    options.SwaggerEndpoint($"/swagger/v{(versionStart + versionStep)}/swagger.json",
                        $"version{versionStart + versionStep} - doc");
                    versionStart += versionStep;
                }

                options.DocExpansion(DocExpansion.None);
            });
        }
    }
}
