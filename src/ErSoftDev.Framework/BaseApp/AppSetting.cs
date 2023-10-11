namespace ErSoftDev.Framework.BaseApp
{
    public class AppSetting
    {
        public string ConnectionString { get; set; }
        public string WelcomeNote { get; set; }
        public Logging Logging { get; set; }
        public Jwt Jwt { get; set; }
        public Swagger? Swagger { get; set; }
        public List<Configuration> Configurations { get; set; }
        public EventLog EventLog { get; set; }
        public EventBusRabbitMq? EventBusRabbitMq { get; set; }
        public NotificationEmail NotificationEmail { get; set; }
        public Redis? Redis { get; set; }
        public Hangfire Hangfire { get; set; }
        public Jaeger? Jaeger { get; set; }
        public ServiceDiscoveryConfig? ServiceDiscoveryConfig { get; set; }
    }

    public class Jaeger
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public double SamplingRate { get; set; }
    }
    public class Logging
    {
        public string AllLevelLog { get; set; }
        public string LogLevel { get; set; }
    }

    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public long TokenExpirySecond { get; set; }
        public long RefreshTokenExpirySecond { get; set; }
        public string? IdentityUrl { get; set; }
    }

    public class Swagger
    {
        public string SpecificationTitle { get; set; }
        public string XmlCommentFileName { get; set; }
        public int VersionCount { get; set; }
    }
    public class Configuration
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class EventLog
    {
        public string AppName { get; set; }
        public int Environment { get; set; }
    }

    public class EventBusRabbitMq
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
        public int VirtualPort { get; set; }
        public int TryCount { get; set; }
        public bool DispatchConsumerAsync { get; set; }
        public string VirtualHost { get; set; }
        public string BrokerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int PreFetchCount { get; set; }
    }

    public class NotificationEmail
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }

    }

    public class Redis
    {
        public List<Hosting> Hosts { get; set; }
        public bool Ssl { get; set; }
        public int ConnectTimeOut { get; set; }
        public int ConnectRetry { get; set; }
        public bool AllowAdmin { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ClientName { get; set; }
    }

    public class Hosting
    {
        public string Host { get; set; }
        public string Port { get; set; }
    }

    public class Hangfire
    {
        public bool UseDatabase { get; set; }
        public int CommandBatchMaxTimeout { get; set; }
        public int SlidingInvisibilityTimeout { get; set; }
        public bool UseRecommendedIsolationLevel { get; set; }
        public bool DisableGlobalLocks { get; set; }
        public bool PrepareSchemaIfNecessary { get; set; }
    }

    public class ServiceDiscoveryConfig
    {
        public string ConsulUrl { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string NameOfService { get; set; }
        public string IdOfService { get; set; }

    }
}
