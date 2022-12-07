using KestrelFramework;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.HttpProxy
{
    /// <summary>
    /// 隧道代理中间件
    /// </summary>
    sealed class TunnelProxyMiddleware : IKestrelMiddleware
    {
        private readonly ILogger<TunnelProxyMiddleware> logger;

        private readonly byte[] http200 = Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection Established\r\n\r\n");
        private readonly byte[] http502 = Encoding.ASCII.GetBytes("HTTP/1.1 502 Bad Gateway\r\n\r\n");

        public TunnelProxyMiddleware(ILogger<TunnelProxyMiddleware> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 隧道代理
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature != null && feature.ProxyProtocol == ProxyProtocol.TunnelProxy)
            {
                await this.ProcessTunnelAsync(context, feature);
            }
            else
            {
                await next(context);
            }
        }

        /// <summary>
        /// 处理隧道代理
        /// </summary>
        /// <param name="context"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        private async ValueTask ProcessTunnelAsync(ConnectionContext context, IProxyFeature feature)
        {
            var output = context.Transport.Output;
            using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var port = feature.ProxyHost.Port;
            if (port == null)
            {
                await output.WriteAsync(http502);
                return;
            }

            try
            {
                var host = feature.ProxyHost.Host;
                await socket.ConnectAsync(host, port.Value, context.ConnectionClosed);
                await output.WriteAsync(http200);
            }
            catch (Exception)
            {
                await output.WriteAsync(http502);
                return;
            }

            this.logger.LogInformation($"隧道代理{feature.ProxyHost}开始");

            var stream = new NetworkStream(socket, ownsSocket: false);
            var task1 = stream.CopyToAsync(output);
            var task2 = context.Transport.Input.CopyToAsync(stream);
            await Task.WhenAny(task1, task2);

            this.logger.LogInformation($"隧道代理{feature.ProxyHost}结束");
        }
    }
}
