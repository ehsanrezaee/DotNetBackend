using ErSoftDev.Framework.Configuration;
using ErSoftDev.Framework.Middlewares;
using ErSoftDev.Framework.RabbitMq;
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
            _appSetting = configuration.GetSection(_configKey).Get<AppSetting>();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {

            services.AddCustomMediatr();
            services.AddGrpc();
            services.AddHttpClient();
            services.Configure<AppSetting>(Configuration.GetSection(_configKey));
            services.AddSingleton(_appSetting);
            services.AddRabbitMqConnection(_appSetting);
            services.AddRabbitMqRegistration(_appSetting);
            //services.AddCustomHangfire(_appSetting);
            services.AddCustomLocalization();
            services.AddMinimalMvc();
            services.AddJwtAuthentication(_appSetting.Jwt);
            services.AddCustomApiVersioning();
            services.AddCustomSwaggerGen(_appSetting.Swagger);
            services.AddControllers();
            services.AddJaeger(_appSetting);
            services.AddCustomConsul(_appSetting);
            //services.AddHangfire(configuration => configuration.UseMediatR());
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, /*IOptions<AppSetting>*/ AppSetting appsetting)
        {
            app.UseCustomRequestLocalization();
            app.UseCustomExceptionMiddleware();
            app.UseHstsNotInDevelopment(env);
            app.UseHttpsRedirection();
            app.UseCustomSwaggerUi(_appSetting.Swagger);
            app.UseAuthentication();
            app.UseMvc();
            app.UseCustomStaticFile();
            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
            //app.UseHangfireDashboard();
            app.UseCustomConsul(appsetting);
            app.UseEndpoints(builder =>
            {
                builder.MapGet("/",
                    async context => { await context.Response.WriteAsync(appsetting./*Value.*/WelcomeNote ?? ""); });
                builder.MapControllers();
                builder.UseGrpcEndPoint();
                //builder.MapHangfireDashboard();
            });

        }
    }
}