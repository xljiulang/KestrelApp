using KestrelFramework;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler.Kestrel
{
    /// <summary>
    /// 隧道传输http中间件
    /// </summary>
    sealed class TunnelHttpMiddleware : IKestrelMiddleware
    {
        private readonly ILogger<TunnelHttpMiddleware> logger;

        public TunnelHttpMiddleware(ILogger<TunnelHttpMiddleware> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 隧道传输http中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature != null && feature.ProxyProtocol == ProxyProtocol.TunnelProxy)
            {
                if (await IsHttpAsync(context))
                {
                    if (context.Features.Get<ITlsConnectionFeature>() == null)
                    {
                        this.logger.LogInformation($"侦测到隧道http流量");
                    }
                    else
                    {
                        this.logger.LogInformation($"侦测到隧道https流量");
                    }
                    await next(context);
                }
                else
                {
                    this.logger.LogInformation($"跳过非http流量{feature.ProxyHost}的隧道拦截");
                    await TunnelAsync(context, feature);
                }
            }
            else
            {
                if (feature?.ProxyProtocol == ProxyProtocol.HttpProxy)
                {
                    this.logger.LogInformation($"侦测到普通http代理流量");
                }
                await next(context);
            }
        }

        /// <summary>
        /// 传输内容是否为http
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async ValueTask<bool> IsHttpAsync(ConnectionContext context)
        {
            var input = context.Transport.Input;
            var result = await input.ReadAtLeastAsync(1);
            var isHttp = HttpDetector.IsHttp(result);
            input.AdvanceTo(result.Buffer.Start);
            return isHttp;
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
                context.Abort();
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
                context.Abort();
            }
        }

        /// <summary>
        /// http协议侦测器
        /// </summary>
        private static class HttpDetector
        {
            private static readonly HttpParser<TunnelRequestHandler> httpParser = new();
            private static readonly TunnelRequestHandler tunnelRequest = new();

            public static bool IsHttp(ReadResult result)
            {
                try
                {
                    var reader = new SequenceReader<byte>(result.Buffer);
                    return httpParser.ParseRequestLine(tunnelRequest, ref reader);
                }
                catch (Exception ex)
                {
                    return ex.Message.Contains("HTTP/");
                }
            }

            private class TunnelRequestHandler : IHttpRequestLineHandler, IHttpHeadersHandler
            {
                public void OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
                {
                }

                public void OnHeadersComplete(bool endStream)
                {
                }

                public void OnStartLine(HttpVersionAndMethod versionAndMethod, TargetOffsetPathLength targetPath, Span<byte> startLine)
                {
                }

                public void OnStaticIndexedHeader(int index)
                {
                }

                public void OnStaticIndexedHeader(int index, ReadOnlySpan<byte> value)
                {
                }
            }
        }
    }
}
