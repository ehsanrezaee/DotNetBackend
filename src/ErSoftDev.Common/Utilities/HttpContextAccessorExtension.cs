using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ErSoftDev.Common.Utilities
{
    public static class HttpContextAccessorExtension
    {
        public static string ClientIp(this IHttpContextAccessor actionContextAccessor)
        {
            return actionContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        public static string LocalIp(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Connection.LocalIpAddress.ToString();
        }

        public static string HostName(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Request.Host.Value;
        }

        public static string Schema(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Request.Scheme;
        }

        public static string? UsernameClaimIdentity(this IHttpContextAccessor accessor)
        {
            if (accessor.HttpContext.User.Identity is ClaimsIdentity identity)
                return identity.GetUserName();

            return "";
        }

        public static string? UserIdClaimIdentity(this IHttpContextAccessor accessor)
        {
            if (accessor.HttpContext.User.Identity is ClaimsIdentity identity)
                return identity.GetUserId();

            return "0";
        }
    }
}
