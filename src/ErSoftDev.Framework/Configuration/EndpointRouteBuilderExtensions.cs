using System.Reflection;
using ErSoftDev.Framework.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ErSoftDev.Framework.Configuration
{
    public static class EndpointRouteBuilderExtensions
    {
        public static void UseGrpcEndPoint(this IEndpointRouteBuilder routeBuilder)
        {
            var grpcServices = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.DefinedTypes)
                .Where(t => typeof(IGrpcService).IsAssignableFrom(t) && !t.IsInterface && t.IsClass && t.IsPublic);

            foreach (var service in grpcServices)
            {
                typeof(GrpcEndpointRouteBuilderExtensions)
                    .GetMethod("MapGrpcService", BindingFlags.Static | BindingFlags.Public)?
                    .MakeGenericMethod(service)
                    .Invoke(null, new object[] { routeBuilder });
            }
        }
    }
}
