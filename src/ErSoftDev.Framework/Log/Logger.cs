﻿using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Tag;
using LogLevel = NLog.LogLevel;

namespace ErSoftDev.Framework.Log
{
    public class Logger<TObject> : ILogger<TObject>
    {
        private readonly ITracer _tracer;
        private readonly IOptions<AppSetting> _appSetting;
        private readonly Microsoft.Extensions.Logging.ILogger<TObject> _microsoftLogger;

        public Logger(ITracer tracer, IOptions<AppSetting> appSetting, Microsoft.Extensions.Logging.ILogger<TObject> microsoftLogger)
        {
            _tracer = tracer;
            _appSetting = appSetting;
            _microsoftLogger = microsoftLogger;
        }

        #region Trace

        public void LogTrace(string message)
        {
            CreateMainLog(message, LogLevel.Trace);
        }
        public void LogTrace(string message, Dictionary<string, string> tags)
        {

            CreateMainLog(message, LogLevel.Trace, tags);
        }
        public void LogTrace<T>(string message, T obj)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Trace);
        }
        public void LogTrace(string message, params object?[] args)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);
            CreateMainLog(string.Format(message, args), LogLevel.Trace);
        }
        public void LogTrace<T>(string message, T obj, Dictionary<string, string> tags)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Trace, tags);
        }
        #endregion

        #region Debug

        public void LogDebug<T>(string message, T obj)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Debug);
        }
        public void LogDebug(string message, params object?[] args)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);
            CreateMainLog(string.Format(message, args), LogLevel.Debug);
        }
        public void LogDebug<T>(string message, T obj, Dictionary<string, string> tags)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Debug, tags);
        }
        public void LogDebug(string message)
        {
            CreateMainLog(message, LogLevel.Debug);
        }
        public void LogDebug(string message, Dictionary<string, string> tags)
        {
            CreateMainLog(message, LogLevel.Debug, tags);
        }
        #endregion

        #region Information
        public void LogInformation<T>(string message, T obj)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Info);
        }
        public void LogInformation<T>(string message, T obj, Dictionary<string, string> tags)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Info, tags);
        }
        public void LogInformation(string message)
        {
            CreateMainLog(message, LogLevel.Info);
        }
        public void LogInformation(string message, params object?[] args)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);
            CreateMainLog(string.Format(message, args), LogLevel.Info);
        }
        public void LogInformation(string message, Dictionary<string, string> tags)
        {
            CreateMainLog(message, LogLevel.Info, tags);
        }
        #endregion

        #region warning
        public void LogWarning<T>(string message, T obj)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Warn);
        }
        public void LogWarning<T>(string message, T obj, Dictionary<string, string> tags)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Warn, tags);
        }
        public void LogWarning(string message)
        {
            CreateMainLog(message, LogLevel.Warn);
        }
        public void LogWarning(string message, params object?[] args)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);
            CreateMainLog(string.Format(message, args), LogLevel.Warn);
        }
        public void LogWarning(string message, Dictionary<string, string> tags)
        {
            CreateMainLog(message, LogLevel.Warn, tags);
        }
        #endregion

        #region Error

        public void LogError<T>(string message, T obj)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Error);
        }
        public void LogError(string message, params object?[] args)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);
            CreateMainLog(string.Format(message, args), LogLevel.Error);
        }
        public void LogError<T>(string message, T obj, Dictionary<string, string> tags)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Error, tags);
        }
        public void LogError(string message)
        {
            CreateMainLog(message, LogLevel.Error);
        }
        public void LogError(string message, Dictionary<string, string> tags)
        {
            CreateMainLog(message, LogLevel.Error, tags);
        }
        #endregion

        #region Fatal

        public void LogFatal<T>(string message, T obj)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Fatal);
        }
        public void LogFatal(string message, params object?[] args)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = JsonConvert.SerializeObject(args[i]);
            CreateMainLog(string.Format(message, args), LogLevel.Fatal);
        }
        public void LogFatal<T>(string message, T obj, Dictionary<string, string> tags)
        {
            CreateMainLog(message + JsonConvert.SerializeObject(obj), LogLevel.Fatal, tags);
        }
        public void LogFatal(string message)
        {
            CreateMainLog(message, LogLevel.Fatal);
        }
        public void LogFatal(string message, Dictionary<string, string> tags)
        {
            CreateMainLog(message, LogLevel.Fatal, tags);
        }
        #endregion

        private void CreateMainLog(string message, LogLevel logLevel, Dictionary<string, string>? tags = null)
        {

            var logLevelFromAppSetting = _appSetting.Value.Logging.LogLevel.ToLower();
            switch (logLevelFromAppSetting)
            {
                case "trace":
                    _microsoftLogger.LogTrace(message);
                    break;

                case "debug":
                    if (logLevel.Name.ToLower() is "trace")
                        return;

                    _microsoftLogger.LogDebug(message);
                    break;

                case "information":
                    if (logLevel.Name.ToLower() is "debug" or "trace")
                        return;

                    _microsoftLogger.LogInformation(message);
                    break;

                case "warning":
                    if (logLevel.Name.ToLower() is "info" or "debug" or "trace")
                        return;

                    _microsoftLogger.LogWarning(message);
                    break;

                case "error":
                    if (logLevel.Name.ToLower() is "warning" or "info" or "debug" or "trace")
                        return;
                    break;

                case "fatal":
                    if (logLevel.Name.ToLower() is "error" or "warning" or "info" or "debug" or "trace")
                        return;

                    _microsoftLogger.LogCritical(message);
                    break;

                case "none":
                    return;
            }



            var current = _tracer
                .BuildSpan(typeof(TObject).FullName?.Substring(
                    typeof(TObject).FullName!.IndexOf(".", StringComparison.Ordinal) + 1))
                .StartActive(true);

            if (current is null)
                return;

            current.Span.Log(DateTimeOffset.Now, message);
            current.Span.SetTag(Tags.SpanKind, Tags.SpanKindClient);

            if (tags != null)
                foreach (var item in tags)
                    current.Span.SetTag(item.Key, item.Value);

            if (logLevel == LogLevel.Error)
                current.Span.SetTag(Tags.Error, true);

            current.Span.Finish(DateTimeOffset.Now);
        }
    }


}
