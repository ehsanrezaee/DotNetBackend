﻿using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace ErSoftDev.Common.Utilities
{
    public static class IdentityExtensions
    {
        public static string? FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity.FindFirst(claimType)?.Value;
        }

        public static string? FindFirstValue(this IIdentity identity, string claimType)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue(claimType);
        }

        public static string? GetUserId(this IIdentity identity)
        {
            return identity.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static T? GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var userId = identity.GetUserId();
            return userId.HasValue()
                ? (T)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture)
                : default(T);
        }

        public static string? GetUserName(this IIdentity identity)
        {
            return identity.FindFirstValue(ClaimTypes.Name);
        }
    }
}
