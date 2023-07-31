using Authy.Common.Entities;
using Azure.Core;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Text.Json;

namespace Authy.API.Middleware
{
    public class JWTCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authenticationCookieName = "authy_jwt";
            context.Request.Cookies.TryGetValue(authenticationCookieName, out string token);

            if (!string.IsNullOrEmpty(token))
            {
                IDbConnectionFactory dbFactory = context.RequestServices.GetService<IDbConnectionFactory>();

                if (dbFactory == null)
                {
                    _next.Invoke(context);
                    return;
                }

                using (var db = dbFactory.OpenDbConnection())
                {
                    var existingKey = await db.SingleWhereAsync<ApiKey>("RefIdStr", token);

                    if (existingKey == null)
                    {
                        await _next.Invoke(context);
                        return;
                    }

                    if (existingKey.CancelledDate.HasValue && existingKey.CancelledDate < DateTime.UtcNow ||
                        existingKey.ExpiryDate.HasValue && existingKey.ExpiryDate < DateTime.UtcNow)
                    {
                        await _next.Invoke(context);
                        return;
                    }
                }

                context.Request.Headers.Append("Authorization", "Bearer " + token);
            }

            await _next.Invoke(context);
        }
    }
}
