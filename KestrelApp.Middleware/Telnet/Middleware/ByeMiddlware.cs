using KestrelFramework.Application;
using System;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Telnet.Middleware
{
    sealed class ByeMiddlware : IApplicationMiddleware<TelnetContext>
    {
        public async Task InvokeAsync(ApplicationDelegate<TelnetContext> next, TelnetContext context)
        {
            if (context.Request.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                await context.Response.WriteLineAsync("Have a good day!");
                context.Abort();
            }
            else
            {
                await next(context);
            }
        }
    }
}
