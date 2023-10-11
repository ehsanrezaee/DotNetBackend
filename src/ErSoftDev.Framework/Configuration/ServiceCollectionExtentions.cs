﻿using System.Reflection;
using System.Security.Claims;
using System.Text;
using ErSoftDev.Common.Utilities;
using ErSoftDev.DomainSeedWork;
using ErSoftDev.Framework.BaseModel;
using ErSoftDev.Framework.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Hangfire;
using Hangfire.SqlServer;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Util;
using OpenTracing.Contrib.NetCore.Configuration;
using StackExchange.Redis;
using ErSoftDev.Framework.BaseApp;
using Consul;

namespace ErSoftDev.Framework.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMinimalMvc(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMvcCore(options =>
                {
                    options.Filters.Add(new AuthorizeFilter());
                    options.EnableEndpointRouting = false;
                    options.SuppressAsyncSuffixInActionNames = false;

                }).AddApiExplorer()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddDataAnnotations()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddCors();
        }

        public static void AddApplicationDbContext(this IServiceCollection serviceCollection, AppSetting appSetting)
        {
            serviceCollection.AddDbContext<BaseDbContext>(options =>
                    {
                        options.UseSqlServer(appSetting.ConnectionString);
                    });
        }

        public static void AddJwtAuthentication(this IServiceCollection services, BaseApp.Jwt jwt)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(HighSecurity.JwtSecretKey);
                var encryptKey = Encoding.UTF8.GetBytes(HighSecurity.JwtEncryptKey);
                var issuer = jwt.Issuer;
                var audience = jwt.Audience;
                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min 
                    RequireSignedTokens = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuer = false,
                    ValidIssuer = issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptKey)
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = async context =>
                     {
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                             throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                 ApiResultErrorCode.TokenIsExpired);

                         if (context.Exception.GetType() != typeof(AppException))
                             throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                 ApiResultErrorCode.TokenIsNotValid);
                     },
                    OnTokenValidated = async context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims?.Any() != true)
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenHasNotClaim);

                        var securityStamp =
                            claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (!securityStamp.HasValue())
                            throw new AppException(new Exception(), ApiResultStatusCode.Failed,
                                ApiResultErrorCode.TokenIsNotSafeWithSecurityStamp);

                    }
                };

            });
        }

        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ReportApiVersions = true;
            });
        }

        public static void AddCustomSwaggerGen(this IServiceCollection serviceCollection, BaseApp.Swagger swagger)
        {
            if (swagger is null)
                return;

            serviceCollection.AddSwaggerGen(options =>
            {
                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, swagger.XmlCommentFileName);

                options.IncludeXmlComments(xmlDocPath, true);
                options.EnableAnnotations();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.CustomSchemaIds(x => x.FullName);

                #region versioning

                var versionCount = swagger.VersionCount;
                decimal versionStart = (decimal)0.9;
                const decimal versionStep = (decimal)0.1;
                for (var i = 0; i < versionCount; i++)
                {
                    options.SwaggerDoc
                    (
                        "v" + (versionStart + versionStep).ToString().Replace('/', '.'),
                        new OpenApiInfo() { Title = swagger.SpecificationTitle, Version = "v" + (versionStart + versionStep).ToString().Replace('/', '.') }
                        );
                    versionStart = versionStart + versionStep;
                }


                options.OperationFilter<RemoveVersionParameter>();

                options.DocumentFilter<SetVersionInPath>();

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo))
                        return false;

                    if (methodInfo.DeclaringType == null)
                        return false;

                    var versions = methodInfo.DeclaringType.GetCustomAttributes<ApiVersionAttribute>(true).SelectMany(attr => attr.Versions);
                    var x = versions.Any(v => $"v{v.ToString()}" == docName);
                    return x;
                });
                #endregion

                #region Add Jwt Authentication

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme.<br>"
                                  + "Get token by access information already sent to your company from \"GetToken\" API.<br>"
                                  + "Use authorize by lock icon in top of page for all action or use authorize of each action. <br>"
                                  + "Add your token to bottom text box with \"bearer[space]\" word in prefix .<br>"
                                  + "For Example : Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header
                });

                options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "bearer");

                #endregion

                #region  EnumDesc
                options.SchemaFilter<EnumSchemaFilter>();
                #endregion

                #region AddParameter
                options.OperationFilter<AddHeaderParameter>();
                #endregion
            });
        }

        public static void AddCustomLocalization(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        public static void AddCustomMediatr(this IServiceCollection serviceCollection)
        {
            var assemblies = Tools.GetAllAssemblies().ToArray();
            serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        }

        public static void AddRedis(this IServiceCollection serviceCollection, BaseApp.Redis appSetting)
        {
            try
            {
                var clientName = appSetting.ClientName;
                var options = new ConfigurationOptions
                {
                    ClientName = clientName,
                    AllowAdmin = appSetting.AllowAdmin,
                    Ssl = appSetting.Ssl,
                    KeepAlive = 5,
                    SyncTimeout = appSetting.ConnectTimeOut,
                    ConnectTimeout = appSetting.ConnectTimeOut,
                    ConnectRetry = appSetting.ConnectRetry,
                    ReconnectRetryPolicy = new LinearRetry(4000),

                };
                if (!string.IsNullOrWhiteSpace(appSetting.User) &&
                    !string.IsNullOrWhiteSpace(appSetting.Password))
                {
                    options.Password = appSetting.Password;
                    options.User = appSetting.User;
                }

                foreach (var host in appSetting.Hosts)
                    options.EndPoints.Add(host.Host, int.Parse(host.Port));

                serviceCollection.AddSingleton(sp => ConnectionMultiplexer.Connect(options));
            }
            catch
            {
                // ignored
            }
        }

        public static void AddJaeger(this IServiceCollection serviceCollection, AppSetting appSetting)
        {
            if (appSetting.Jaeger is null)
                return;

            serviceCollection.AddSingleton<ITracer>(sp =>
            {
                var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var sampler = new ProbabilisticSampler(appSetting.Jaeger.SamplingRate);

                var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(loggerFactory)
                    .WithSender(new UdpSender(appSetting.Jaeger.Host, appSetting.Jaeger.Port, 0))
                    .Build();

                var tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .WithReporter(reporter)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });
            serviceCollection.AddOpenTracing();
            serviceCollection.Configure<HttpHandlerDiagnosticOptions>(options =>
            {
                options.OperationNameResolver =
                    request => (request.Method.Method.ToLower() == "post")
                        ? request?.RequestUri?.AbsoluteUri.Split("/".ToCharArray()).Last()
                        : request?.RequestUri?.AbsoluteUri.Split("/".ToCharArray()).Last()
                            .Split("?")[0];
            });
        }

        public static void AddCustomHangfire(this IServiceCollection services, AppSetting appSetting)
        {

            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .UseSqlServerStorage(appSetting.ConnectionString, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(appSetting.Hangfire.CommandBatchMaxTimeout),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(appSetting.Hangfire.SlidingInvisibilityTimeout),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = appSetting.Hangfire.UseRecommendedIsolationLevel,
                        DisableGlobalLocks = appSetting.Hangfire.DisableGlobalLocks,
                        PrepareSchemaIfNecessary = appSetting.Hangfire.PrepareSchemaIfNecessary
                    })
                    .UseMediatR()
            );

            services.AddHangfireServer();

        }

        public static void AddCustomConsul(this IServiceCollection serviceCollection, AppSetting appSetting)
        {
            if (appSetting.ServiceDiscoveryConfig is null)
                return;

            serviceCollection.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(appSetting.ServiceDiscoveryConfig.ConsulUrl);
            }));
        }
    }
}
