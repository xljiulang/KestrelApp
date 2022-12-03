using KestrelApp.TlsDetect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KestrelApp
{
    public static class ListenOptionsExtensions
    {
        /// <summary>
        /// 使用Tls侦测中间件
        /// 效果是同一个端口支持某协议的非tls和tls两种流量
        /// </summary>
        /// <param name="listen"></param>
        /// <param name="configure">https配置</param>
        /// <returns></returns>
        public static ListenOptions UseTlsDetect(this ListenOptions listen, Action< HttpsConnectionAdapterOptions> configure)
        {
            var invadeMiddleware = listen.ApplicationServices.GetRequiredService<TlsInvadeMiddleware>();
            var restoreMiddleware = listen.ApplicationServices.GetRequiredService<TlsRestoreMiddleware>();

            listen.Use(next => context => invadeMiddleware.InvokeAsync(next, context));
            listen.UseHttps(configure);
            listen.Use(next => context => restoreMiddleware.InvokeAsync(next, context));
            return listen;
        }
    }
}
