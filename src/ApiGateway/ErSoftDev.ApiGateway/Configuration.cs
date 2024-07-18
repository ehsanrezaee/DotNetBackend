using ErSoftDev.ApiGateway.Application.HealthChecks;
using ErSoftDev.ApiGateway.Extensions;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using ErSoftDev.Identity.EndPoint.Grpc.Protos;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Ocelot.Middleware;

namespace ErSoftDev.ApiGateway
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
            _appSetting = appConfiguration.GetSection(_configKey).Get<AppSetting>();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddGrpcClient<AccountGrpcService.AccountGrpcServiceClient>(options =>
                options.Address = new Uri(_appSetting.Jwt.IdentityUrl!));

            services.AddHttpContextAccessor();
            services.Configure<AppSetting>(AppConfiguration.GetSection(_configKey));
            services.AddGrpc();
            services.AddSingleton(_appSetting);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddCustomApiGatewayJwtAuthentication(_appSetting.Jwt);
            services.AddCustomLocalization();
            services.AddJaeger(_appSetting);
            services.AddHealthChecks().AddCheck<IdentityGrpcServiceHealthCheck>("IdentityGrpcServiceHealthCheck");


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSetting appsetting)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            ServiceCollectionExtension.Configure(httpContextAccessor);

            app.UseRouting();
            app.UseCustomExceptionMiddleware();
            app.UseCustomRequestLocalization();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseAuthentication();
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseAuthorization();
            app.UseSwaggerForOcelotUI(options => { options.PathToSwaggerGenerator = "/swagger/docs"; }).UseOcelot()
                .Wait();
            app.UseEndpoints(builder =>
            {
                builder.MapGet("/",
                    async context => { await context.Response.WriteAsync(appsetting.WelcomeNote ?? ""); });
                builder.MapControllers();
            });
        }
    }
}
