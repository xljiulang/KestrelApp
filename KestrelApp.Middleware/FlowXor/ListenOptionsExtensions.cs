using KestrelApp.Middleware.FlowXor;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// ListenOptions扩展
    /// </summary>
    public static partial class ListenOptionsExtensions
    {        
        /// <summary>
        /// 使用Xor处理流量
        /// </summary>
        /// <param name="listen"></param>
        /// <returns></returns>
        public static ListenOptions UseFlowXor(this ListenOptions listen)
        {
            listen.Use(next => async context =>
            {
                var oldTransport = context.Transport;
                try
                {
                    await using var duplexPipe = new XorDuplexPipe(context.Transport);
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
