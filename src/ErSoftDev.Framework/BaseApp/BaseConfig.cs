using ErSoftDev.Framework.Configuration;
using ErSoftDev.Framework.Middlewares;
using ErSoftDev.Framework.RabbitMq;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ErSoftDev.Framework.BaseApp
{
    public class BaseConfig
    {
        private readonly AppSetting _appSetting;
        private readonly string _configKey;
        public IConfiguration Configuration { get; }

        public BaseConfig(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            _configKey = $"{nameof(AppSetting)}{environment.EnvironmentName}";
            _appSetting = configuration.GetSection(_configKey).Get<AppSetting>()!;
        }

        public virtual void ConfigureServices(IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddCustomMediatr();
            services.AddGrpc();
            services.AddHttpClient();
            services.Configure<AppSetting>(Configuration.GetSection(_configKey));
            services.AddSingleton(_appSetting);
            services.AddRabbitMqConnection(_appSetting);
            services.AddRabbitMqRegistration(_appSetting);
            services.AddCustomHangfire(_appSetting);
            services.AddCustomLocalization();
            services.AddMinimalMvc();
            services.AddJwtAuthentication(_appSetting.Jwt);
            services.AddCustomApiVersioning();
            services.AddCustomSwaggerGen(_appSetting.Swagger);
            services.AddControllers();
            services.AddJaeger(_appSetting);
            services.AddCustomConsul(_appSetting);
            services.AddCustomIdGenerator();
            services.AddCustomCap(_appSetting);
            services.AddHealthChecks().AddCustomCheck();
            services.AddCustomLogging(_appSetting, Configuration, environment);
            services.AddHttpContextAccessor();
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSetting appSetting)
        {
            app.UseRouting();
            app.UseCustomRequestLocalization();
            app.UseCustomStringLocalizer();
            app.UseCustomExceptionMiddleware();
            app.UseCustomJwtTokenValidation();
            app.UseHstsNotInDevelopment(env);
            app.UseHttpsRedirection();
            app.UseCustomHealthCheck();
            app.UseCustomSwaggerUi(_appSetting.Swagger);
            app.UseAuthentication();
            app.UseMvc();
            app.UseCustomStaticFile();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            app.UseCustomConsul(appSetting);
            app.UseCustomHangFireDashboard(appSetting);
            app.UseEndpoints(builder =>
            {
                builder.MapGet("/",
                    async context => { await context.Response.WriteAsync(appSetting.WelcomeNote ?? ""); });
                builder.MapControllers();
                builder.UseGrpcEndPoint();
                builder.MapHangfireDashboard();
            });

        }
    }
}