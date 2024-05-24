using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Identity.Application.IntegrationEvents;
using ErSoftDev.Identity.Infrastructure;
using EventBus.Base.Standard.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ErSoftDev.Identity.EndPoint
{
    public class Configuration : BaseConfig
    {

        private readonly AppSetting _appSetting;
        public Configuration(IConfiguration configuration, IHostEnvironment environment) : base(configuration, environment)
        {
            _appSetting = configuration.GetSection($"{nameof(AppSetting)}{environment.EnvironmentName}")
                .Get<AppSetting>()!;
        }

        public override void ConfigureServices(IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.AddDbContext<IdentityDbContext>(builder =>
                builder.UseSqlServer(_appSetting.ConnectionString.AppConnectionString));
            services.AddDbContext<IdentityQueryDbContext>(builder =>
                builder.UseSqlServer(_appSetting.ConnectionString.AppConnectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddEventBusHandling(IntegrationEventExtension.GetHandlers());
            base.ConfigureServices(services, webHostEnvironment);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSetting appsetting)
        {
            app.SubscribeToEvents();
            base.Configure(app, env, appsetting);
        }
    }
}
