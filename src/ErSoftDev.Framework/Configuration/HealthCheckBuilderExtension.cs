using ErSoftDev.Common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ErSoftDev.Framework.Configuration
{
    public static class HealthCheckBuilderExtension
    {
        public static void AddCustomCheck(this IHealthChecksBuilder healthChecksBuilder)
        {
            var assemblies = Tools.GetAllAssemblies();
            var healthCheckClasses =
                assemblies
                    .SelectMany(a => a.DefinedTypes)
                    .Where(t => typeof(IHealthCheck).IsAssignableFrom(t) && !t.IsInterface && t.IsClass && t.IsPublic);

            foreach (var healthCheckClass in healthCheckClasses)
            {
                typeof(HealthChecksBuilderAddCheckExtensions).GetMethods().FirstOrDefault(
                        x => x.Name.Equals("AddCheck", StringComparison.OrdinalIgnoreCase) &&
                             x.IsGenericMethod)
                    ?.MakeGenericMethod(healthCheckClass)
                    .Invoke(null, new object[] { healthChecksBuilder, healthCheckClass.Name, null, null });
            }
        }
    }
}
