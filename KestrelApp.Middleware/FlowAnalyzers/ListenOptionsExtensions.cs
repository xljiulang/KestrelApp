using KestrelApp.Middleware.FlowAnalyzers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// ListenOptions扩展
    /// </summary>
    public static partial class ListenOptionsExtensions
    {
        /// <summary>
        /// 使用流量分析中间件
        /// </summary>
        /// <param name="listen"></param>
        /// <returns></returns>
        public static ListenOptions UseFlowAnalyze(this ListenOptions listen)
        {
            var flowAnalyzer = listen.ApplicationServices.GetRequiredService<IFlowAnalyzer>();
            listen.Use(next => async context =>
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
            });
            return listen;
        } 
    }
}
