using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Configuration = ErSoftDev.Identity.EndPoint.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(
    (context, containerBuilder) => containerBuilder.RegisterModule(new AutofacConfigurationExtension()));

var appSettings = builder.Configuration
    .GetSection($"{nameof(AppSetting)}{builder.Environment.EnvironmentName}")
    .Get<AppSetting>();

var startup = new Configuration(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services, builder.Environment);

var app = builder.Build();

startup.Configure(app, builder.Environment, appSettings);

await app.RunAsync();