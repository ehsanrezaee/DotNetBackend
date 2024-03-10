using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Identity.Application.IntegrationEvents;
using ErSoftDev.Identity.Infrastructure;
using EventBus.Base.Standard.Configuration;

namespace ErSoftDev.Identity.EndPoint
{
    public class Configuration : BaseConfig
    {
        public Configuration(IConfiguration configuration, IHostEnvironment environment) : base(configuration, environment)
        {

        }

        public override void ConfigureServices(IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.AddDbContext<IdentityDbContext>();
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
