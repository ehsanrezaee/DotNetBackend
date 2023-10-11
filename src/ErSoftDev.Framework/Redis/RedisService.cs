using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using ErSoftDev.Framework.Log;
using ErSoftDev.Framework.RabbitMq;
using EventBus.Base.Standard;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace ErSoftDev.Framework.Redis
{
    public class RedisService : IScopedDependency, IRedisService
    {
        private readonly IOptions<AppSetting> _appSetting;
        private readonly IEventBus _eventBus;
        private readonly ILogger<RedisService> _logger;

        private ConnectionMultiplexer _connectionMultiplexer;
        //private readonly IConnectionMultiplexer _connectionMultiplexer;
        //private readonly IDatabaseAsync _databaseAsync;

        private IDatabase _db;
        private string _clientName;

        public RedisService(IOptions<AppSetting> appSetting,
            IEventBus eventBus, ILogger<RedisService> logger/*, IConnectionMultiplexer connectionMultiplexer*/)
        {
            _appSetting = appSetting;
            _eventBus = eventBus;
            _logger = logger;
            //_connectionMultiplexer = connectionMultiplexer;
            //_databaseAsync = _connectionMultiplexer.GetDatabase();
            Config();
        }

        private void Config()
        {
            try
            {
                if (_appSetting.Value.Redis is null)
                    return;

                _clientName = _appSetting.Value.Redis.ClientName;
                var options = new ConfigurationOptions
                {
                    ClientName = _clientName,
                    AllowAdmin = _appSetting.Value.Redis.AllowAdmin,
                    Ssl = _appSetting.Value.Redis.Ssl,
                    KeepAlive = 5,
                    SyncTimeout = _appSetting.Value.Redis.ConnectTimeOut,
                    ConnectTimeout = _appSetting.Value.Redis.ConnectTimeOut,
                    ConnectRetry = _appSetting.Value.Redis.ConnectRetry,
                    ReconnectRetryPolicy = new LinearRetry(4000),

                };
                if (!string.IsNullOrWhiteSpace(_appSetting.Value.Redis.User) &&
                    !string.IsNullOrWhiteSpace(_appSetting.Value.Redis.Password))
                {
                    options.Password = _appSetting.Value.Redis.Password;
                    options.User = _appSetting.Value.Redis.User;
                }

                foreach (var host in _appSetting.Value.Redis.Hosts)
                    options.EndPoints.Add(host.Host, int.Parse(host.Port));
                try
                {
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(options);
                    _connectionMultiplexer.ConnectionFailed += OnConnectionFailed;
                    _connectionMultiplexer.InternalError += OnInternalError;
                    _connectionMultiplexer.ConfigurationChanged += OnConfigurationChanged;

                    _db = _connectionMultiplexer.GetDatabase();
                }
                catch (Exception e)
                {
                    _logger.LogInformation("RedisConstructor", "Constructor connection not happen",
                        new { _appSetting.Value.Redis.ClientName, e.Message });
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisConstructor", "Exception in Constructor config",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
            }

        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogInformation("RedisOnConnectionFailedEvent", "Connection failed",
                new { e.EndPoint, _appSetting.Value.Redis.ClientName, e.Exception });

            _eventBus.Publish(new SendEmailIntegrationEvent()
            {
                From = _appSetting.Value.NotificationEmail.Sender,
                Subject = "Redis connection is aborted",
                To = _appSetting.Value.NotificationEmail.Recipient,
                Message = $"Redis server : {e.EndPoint}/n " +
                          $", Client name : {_appSetting.Value.Redis.ClientName} /n " +
                          $", Exception Message :{e.Exception.Message} /n"
            });
        }

        private void OnInternalError(object sender, InternalErrorEventArgs e)
        {

        }

        private void OnConfigurationChanged(object sender, EndPointEventArgs e)
        {

        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        public async Task<bool> AddOrUpdateAsync(string key, string value,
            ExpiryTime expiry = ExpiryTime.TenMinute, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await _db.StringSetAsync(_clientName + ":" + key, value, TimeSpan.FromSeconds((long)expiry), When.Always,
                    flags);
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisAddOrUpdateAsync", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return false;
            }
        }

        /// <summary>
        /// Set key to hold the string value for exactly second time. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expireTimeSecond">The exactly second time</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        public async Task<bool> AddOrUpdateAsync(string key, string value,
            long expireTimeSecond = 60, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await _db.StringSetAsync(_clientName + ":" + key, value, TimeSpan.FromSeconds(expireTimeSecond),
                    When.Always,
                    flags);
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisAddOrUpdateAsync", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return false;
            }
        }


        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">Generic class that convert to json</param>
        /// <param name="expiry">The expiry to set.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        public async Task<bool> AddOrUpdateAsync<T>(string key, T value, ExpiryTime expiry = ExpiryTime.TenMinute,
            CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await _db.StringSetAsync(_clientName + ":" + key,
                    JsonConvert.SerializeObject(value, Formatting.Indented,
                        new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    TimeSpan.FromSeconds((long)expiry), When.Always,
                    flags);
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisAddOrUpdateAsync<T>", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return false;
            }
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <param name="value">Generic class that convert to json</param>
        /// <param name="expireTimeSecond">The exactly second time</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the string was set, false otherwise.</returns>
        public async Task<bool> AddOrUpdateAsync<T>(string key, T value, long expireTimeSecond = 60,
            CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await _db.StringSetAsync(_clientName + ":" + key,
                    JsonConvert.SerializeObject(value, Formatting.Indented,
                        new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    TimeSpan.FromSeconds(expireTimeSecond), When.Always,
                    flags);
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisAddOrUpdateAsync<T>", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return false;
            }
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        public async Task<string> GetAsync(string key)
        {
            try
            {
                var result = await _db.StringGetAsync(_clientName + ":" + key);
                return result.ToString();
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisGetAsync", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return null;
            }
        }

        /// <summary>
        /// Get the T class value of key. If the key does not exist the special value nil is returned. An error is returned if the value stored at key is not a string, because GET only handles string values.
        /// </summary>
        /// <param name="key">The key of the string.</param>
        /// <returns>The value of key, or nil when key does not exist.</returns>
        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var result = await _db.StringGetAsync(_clientName + ":" + key);
                return result.ToString() == null ? default : JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisGetAsync<T>", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return default;
            }
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the key was removed.</returns>
        public async Task<bool> DeleteAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await _db.KeyDeleteAsync(_clientName + ":" + key, flags);
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisDeleteAsync", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return false;
            }
        }

        /// <summary>
        /// Removes all keys that matched with pattern.
        /// </summary>
        /// <param name="key">The key to delete. pattern is key*</param>
        /// <param name="flags">The flags to use for this operation.</param>
        /// <returns>True if the key was removed.</returns>
        public async Task<bool> DeleteWithLikeAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                var endPoints = _connectionMultiplexer.GetEndPoints();
                var keys = new List<RedisKey>();
                foreach (var endPoint in endPoints)
                    keys.AddRange(_connectionMultiplexer.GetServer(endPoint)
                        .Keys(-1, _clientName + ":" + key + "*").ToList());

                foreach (var item in keys)
                    await _db.KeyDeleteAsync(item, flags);

                return true;
            }
            catch (Exception e)
            {
                _logger.LogInformation("RedisDeleteAsync", "Exception happen",
                    new { _appSetting.Value.Redis.ClientName, e.Message });
                return false;
            }
        }
    }

    #region SendNotificationEvent
    public class SendEmailIntegrationEvent : BasicEvent
    {
        public SendEmailIntegrationEvent() : base(Queue.MessageQueue)
        {
        }
        public string Subject { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
        public List<FileInformation> ListFile { get; set; }
        public string To { get; set; }
    }
    public class FileInformation
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }
    #endregion

    public enum ExpiryTime
    {
        OneMinute = 60,
        FiveMinute = 5 * 60,
        TenMinute = 10 * 60,
        FifteenMinute = 15 * 60,
        ThirtyMinute = 30 * 60,
        OneHour = 60 * 60,
        TwoHour = 2 * 60 * 60,
        OneDay = 24 * 60 * 60,
        TwoDay = 2 * 24 * 60 * 60,
        OneMonth = 30 * 24 * 60 * 60,
        OneYear = 365 * 24 * 60 * 60,
    }

}
