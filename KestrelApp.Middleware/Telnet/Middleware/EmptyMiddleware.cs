using KestrelFramework.Application;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Telnet.Middleware
{
    sealed class EmptyMiddleware : IApplicationMiddleware<TelnetContext>
    {
        public async Task InvokeAsync(ApplicationDelegate<TelnetContext> next, TelnetContext context)
        {
            if (string.IsNullOrEmpty(context.Request))
            {
                await context.Response.WriteLineAsync("Please type something.");
            }
            else
            {
                await next(context);
            }
        }
    }
}
