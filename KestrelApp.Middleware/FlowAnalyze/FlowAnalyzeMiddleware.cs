using KestrelFramework;
using Microsoft.AspNetCore.Connections;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.FlowAnalyze
{
    sealed class FlowAnalyzeMiddleware : IKestrelMiddleware
    {
        private readonly IFlowAnalyzer flowAnalyzer;

        public FlowAnalyzeMiddleware(IFlowAnalyzer flowAnalyzer)
        {
            this.flowAnalyzer = flowAnalyzer;
        }

        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            var oldTransport = context.Transport;
            try
            {
                await using var duplexPipe = new FlowAnalyzeDuplexPipe(context.Transport, flowAnalyzer);
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
