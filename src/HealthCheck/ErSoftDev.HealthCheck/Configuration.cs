using ErSoftDev.Framework.BaseApp;

namespace ErSoftDev.HealthCheck
{
    public class Configuration
    {
        private readonly AppSetting _appSetting;
        private readonly string _configKey;
        public IConfiguration AppConfiguration { get; }

        public Configuration(IConfiguration appConfiguration, IHostEnvironment environment)
        {
            AppConfiguration = appConfiguration;
            _configKey = $"{nameof(AppSetting)}{environment.EnvironmentName}";
            _appSetting = appConfiguration.GetSection(_configKey).Get<AppSetting>()!;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSetting>(AppConfiguration.GetSection(_configKey));
            //services.AddSingleton(_appSetting);
            services.AddControllers();
            services.AddHealthChecksUI()
                .AddInMemoryStorage();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSetting appsetting)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapGet("/",
                    async context => { await context.Response.WriteAsync(appsetting.WelcomeNote ?? ""); });
                builder.MapControllers();
            });
            app.UseHealthChecksUI(config => config.UIPath = "/healthCheckUI");
        }
    }
}
