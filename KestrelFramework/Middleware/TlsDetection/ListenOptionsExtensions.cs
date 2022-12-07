using KestrelFramework.Middleware.TlsDetection;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// ListenOptions扩展
    /// </summary>
    public static partial class ListenOptionsExtensions
    {
        /// <summary>
        /// 使用Tls侦测中间件
        /// 效果是同一个端口支持某协议的非tls和tls两种流量
        /// </summary>
        /// <param name="listen"></param> 
        /// <returns></returns>
        public static ListenOptions UseTlsDetection(this ListenOptions listen)
        {
            return listen.UseTlsDetection(options => { });
        }

        /// <summary>
        /// 使用Tls侦测中间件
        /// 效果是同一个端口支持某协议的非tls和tls两种流量
        /// </summary>
        /// <param name="listen"></param>
        /// <param name="configure">tls配置</param>
        /// <returns></returns>
        public static ListenOptions UseTlsDetection(this ListenOptions listen, Action<HttpsConnectionAdapterOptions> configure)
        {
            listen.Use<TlsInvadeMiddleware>();
            listen.UseHttps(configure);
            listen.Use<TlsRestoreMiddleware>();
            return listen;
        }
    }
}
