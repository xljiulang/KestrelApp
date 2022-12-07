using KestrelFramework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.FlowXor
{
    sealed class XorMiddleware : IKestrelMiddleware
    {
        private readonly ILogger<XorMiddleware> logger;

        public XorMiddleware(ILogger<XorMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            var oldTransport = context.Transport;
            try
            {
                await using var duplexPipe = new XorDuplexPipe(context.Transport, this.logger);
                context.Transport = duplexPipe;
                await next(context);
            }
            finally
            {
                context.Transport = oldTransport;
            }
        }
    }
}
