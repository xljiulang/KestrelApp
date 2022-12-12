using KestrelFramework;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler.Middlewares
{
    /// <summary>
    /// 隧道传输中间件
    /// </summary>
    sealed class KestrelTunnelMiddleware : IKestrelMiddleware
    {
        private readonly ILogger<KestrelTunnelMiddleware> logger;

        /// <summary>
        /// 隧道传输中间件
        /// </summary>
        /// <param name="logger"></param>
        public KestrelTunnelMiddleware(ILogger<KestrelTunnelMiddleware> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 执行中间你件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature == null || feature.ProxyProtocol == ProxyProtocol.None)
            {
                this.logger.LogInformation($"侦测到http直接请求");
                await next(context);
            }
            else if (feature.ProxyProtocol == ProxyProtocol.HttpProxy)
            {
                this.logger.LogInformation($"侦测到普通http代理流量");
                await next(context);
            }
            else if (await FlowDetector.IsHttpAsync(context))
            {
                this.logger.LogInformation($"侦测到隧道传输http流量");
                await next(context);
            }
            else
            {
                this.logger.LogInformation($"跳过隧道传输非http流量{feature.ProxyHost}的拦截");
                await TunnelAsync(context, feature);
            }
        }

        /// <summary>
        /// 隧道传输其它协议的数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        private async ValueTask TunnelAsync(ConnectionContext context, IProxyFeature feature)
        {
            var port = feature.ProxyHost.Port;
            if (port == null)
            {
                return;
            }

            try
            {
                var host = feature.ProxyHost.Host;
                using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(host, port.Value, context.ConnectionClosed);
                Stream stream = new NetworkStream(socket, ownsSocket: false);

                // 如果有tls中间件，则反回来加密隧道
                if (context.Features.Get<ITlsConnectionFeature>() != null)
                {
                    var sslStream = new SslStream(stream, leaveInnerStreamOpen: true);
                    await sslStream.AuthenticateAsClientAsync(feature.ProxyHost.Host);
                    stream = sslStream;
                }

                var task1 = stream.CopyToAsync(context.Transport.Output);
                var task2 = context.Transport.Input.CopyToAsync(stream);
                await Task.WhenAny(task1, task2);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"连接到{feature.ProxyHost}异常");
            }
        }

        /// <summary>
        /// 流量侦测器
        /// </summary>
        private static class FlowDetector
        {
            private static readonly byte[] crlf = Encoding.ASCII.GetBytes("\r\n");
            private static readonly byte[] http10 = Encoding.ASCII.GetBytes(" HTTP/1.0");
            private static readonly byte[] http11 = Encoding.ASCII.GetBytes(" HTTP/1.1");
            private static readonly byte[] http20 = Encoding.ASCII.GetBytes(" HTTP/2.0");

            /// <summary>
            /// 传输内容是否为http
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public static async ValueTask<bool> IsHttpAsync(ConnectionContext context)
            {
                var input = context.Transport.Input;
                var result = await input.ReadAtLeastAsync(1);
                var isHttp = IsHttp(result);
                input.AdvanceTo(result.Buffer.Start);
                return isHttp;
            }

            private static bool IsHttp(ReadResult result)
            {
                var reader = new SequenceReader<byte>(result.Buffer);
                if (reader.TryReadToAny(out ReadOnlySpan<byte> line, crlf))
                {
                    return line.EndsWith(http11) || line.EndsWith(http20) || line.EndsWith(http10);
                }
                return false;
            }
        }
    }
}
