using System.Globalization;
using Consul;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ErSoftDev.Framework.Configuration
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseHstsNotInDevelopment(this IApplicationBuilder applicationBuilder, IWebHostEnvironment hostingEnvironment)
        {
            if (!hostingEnvironment.IsDevelopment())
                applicationBuilder.UseHsts();
        }

        public static void UseCustomRequestLocalization(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("fa-IR"),
                SupportedCultures = new[]
                {
                    new CultureInfo("fa-IR"),
                    new CultureInfo("en-US")
                },

            });
        }
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }

        public static void UseCustomStringLocalizer(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomStringLocalizedMiddleware>();
        }

        public static void UseRateLimitationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RateLimitationMiddleware>();
        }

        public static void UseCustomStaticFile(this IApplicationBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".log"] = "text/html";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });
        }

        public static IApplicationBuilder UseCustomConsul(this IApplicationBuilder app, AppSetting appSetting)
        {

            if (appSetting.ServiceDiscoveryConfig is null)
                return app;

            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var lifetime = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Hosting.IApplicationLifetime>();

            if (!(app.Properties["server.Features"] is FeatureCollection features))
                return app;

            var registration = new AgentServiceRegistration()
            {
                ID = appSetting.ServiceDiscoveryConfig.IdOfService,
                Name = appSetting.ServiceDiscoveryConfig.NameOfService,
                Address = appSetting.ServiceDiscoveryConfig.Host,
                Port = appSetting.ServiceDiscoveryConfig.Port,
                Tags = new[] { $"urlprefix-/{appSetting.ServiceDiscoveryConfig.IdOfService}" }
            };

            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            });

            return app;
        }
    }
}
