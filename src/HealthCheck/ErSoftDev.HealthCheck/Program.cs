using Autofac;
using Autofac.Extensions.DependencyInjection;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(
    (context, containerBuilder) => containerBuilder.RegisterModule(new AutofacConfigurationExtension()));

var appSettings = builder.Configuration
    .GetSection($"{nameof(AppSetting)}{builder.Environment.EnvironmentName}")
    .Get<AppSetting>();

var startup = new ErSoftDev.HealthCheck.Configuration(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, builder.Environment, appSettings);

await app.RunAsync();