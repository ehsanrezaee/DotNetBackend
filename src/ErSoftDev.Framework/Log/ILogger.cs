namespace ErSoftDev.Framework.Log
{
    public interface ILogger<TObject>
    {
        void LogTrace(string message);
        void LogTrace(string message, Dictionary<string, string> tags);
        void LogTrace<T>(string message, T obj);
        void LogTrace(string message, params object[] args);
        void LogTrace<T>(string message, T obj, Dictionary<string, string> tags);
        void LogDebug<T>(string message, T obj);
        void LogDebug(string message, params object[] args);
        void LogDebug<T>(string message, T obj, Dictionary<string, string> tags);
        void LogDebug(string message);
        void LogDebug(string message, Dictionary<string, string> tags);
        void LogInformation<T>(string message, T obj);
        void LogInformation<T>(string message, T obj, Dictionary<string, string> tags);
        void LogInformation(string message);
        void LogInformation(string message, params object[] args);
        void LogInformation(string message, Dictionary<string, string> tags);
        void LogWarning<T>(string message, T obj);
        void LogWarning<T>(string message, T obj, Dictionary<string, string> tags);
        void LogWarning(string message);
        void LogWarning(string message, params object[] args);
        void LogWarning(string message, Dictionary<string, string> tags);
        void LogError<T>(string message, T obj);
        void LogError(string message, params object[] args);
        void LogError<T>(string message, T obj, Dictionary<string, string> tags);
        void LogError(string message);
        void LogError(string message, Dictionary<string, string> tags);
        void LogFatal<T>(string message, T obj);
        void LogFatal(string message, params object[] args);
        void LogFatal<T>(string message, T obj, Dictionary<string, string> tags);
        void LogFatal(string message);
        void LogFatal(string message, Dictionary<string, string> tags);
    }

}