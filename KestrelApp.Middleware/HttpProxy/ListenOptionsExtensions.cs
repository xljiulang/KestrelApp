using KestrelApp.Middleware.HttpProxy;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
        public static ListenOptions UseHttpProxy(this ListenOptions listen)
        {
            listen.Use<ProxyMiddleware>();
            listen.Use<TunnelProxyMiddleware>();
            return listen;
        }
    }
}
