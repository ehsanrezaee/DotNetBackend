﻿using ErSoftDev.ApiGateway.Extensions;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using ErSoftDev.Identity.EndPoint.Grpc.Protos;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

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

            services.Configure<AppSetting>(AppConfiguration.GetSection(_configKey));
            services.AddGrpc();
            services.AddSingleton(_appSetting);
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddCustomApiGatewayJwtAuthentication(_appSetting.Jwt);
            services.AddCustomLocalization();
            services.AddJaeger(_appSetting);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSetting appsetting)
        {
            app.UseSwagger();
            app.UseCustomRequestLocalization();
            app.UseCustomExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSwaggerForOcelotUI(options =>
            {
                options.PathToSwaggerGenerator = "/swagger/docs";

            }).UseOcelot().Wait();
            app.UseEndpoints(builder =>
            {
                builder.MapGet("/",
                    async context => { await context.Response.WriteAsync(appsetting.WelcomeNote ?? ""); });
                builder.MapControllers();
            });
        }
    }
}
