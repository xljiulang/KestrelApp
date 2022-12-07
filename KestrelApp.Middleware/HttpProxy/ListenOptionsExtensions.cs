using KestrelApp.Middleware.HttpProxy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    ///  ListenOptions扩展
    /// </summary>
    public static partial class ListenOptionsExtensions
    {
        /// <summary>
        /// 使用http代理中间件
        /// </summary>
        /// <param name="listen"></param>
        public static void UseHttpProxy(this ListenOptions listen)
        {
            var proxyMiddleware = listen.ApplicationServices.GetRequiredService<ProxyMiddleware>();
            listen.Use(next => context => proxyMiddleware.InvokeAsync(next, context));
        }
    }
}
