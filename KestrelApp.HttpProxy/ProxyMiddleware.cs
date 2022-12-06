using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.HttpProxy
{
    /// <summary>
    /// 代理中间件
    /// </summary>
    sealed class ProxyMiddleware
    {
        private readonly HttpParser<HttpRequestHandler> httpParser = new();
        private readonly byte[] http200 = Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection Established\r\n\r\n");
        private readonly byte[] http400 = Encoding.ASCII.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n");

        private readonly ILogger<ProxyMiddleware> logger;

        /// <summary>
        /// 代理中间件
        /// </summary>
        /// <param name="logger"></param>
        public ProxyMiddleware(ILogger<ProxyMiddleware> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 解析代理
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            var input = context.Transport.Input;
            var output = context.Transport.Output;
            var request = new HttpRequestHandler();

            while (context.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                try
                {
                    if (this.ParseRequest(result, request, out var consumed))
                    {
                        context.Features.Set<IProxyFeature>(request);
                        if (request.ProxyProtocol == ProxyProtocol.TunnelProxy)
                        {
                            input.AdvanceTo(consumed);
                            await this.ProcessTunnelAsync(context, request);
                        }
                        else
                        {
                            input.AdvanceTo(result.Buffer.Start);
                            await next(context);
                        }
                        break;
                    }
                    else
                    {
                        input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                    }

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    await output.WriteAsync(this.http400, context.ConnectionClosed);
                    break;
                }
            }
        }


        private async ValueTask ProcessTunnelAsync(ConnectionContext context, HttpRequestHandler request)
        {
            try
            {
                using var tunnel = new TunnelProxyConnection(request);
                await tunnel.ConnectAsync(context.ConnectionClosed);
                await context.Transport.Output.WriteAsync(this.http200, context.ConnectionClosed);

                this.logger.LogInformation($"隧道代理{request.ProxyHost}开始");
                await tunnel.TransportAsync(context);
                this.logger.LogInformation($"隧道代理{request.ProxyHost}结束");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"无法代理{request.ProxyHost}");
            }
        }

        /// <summary>
        /// 解析http请求
        /// </summary>
        /// <param name="result"></param>
        /// <param name="requestHandler"></param>
        /// <param name="consumed"></param>
        /// <returns></returns>
        private bool ParseRequest(ReadResult result, HttpRequestHandler request, out SequencePosition consumed)
        {
            var reader = new SequenceReader<byte>(result.Buffer);
            if (this.httpParser.ParseRequestLine(request, ref reader) &&
                this.httpParser.ParseHeaders(request, ref reader))
            {
                consumed = reader.Position;
                return true;
            }
            else
            {
                consumed = default;
                return false;
            }
        }


        /// <summary>
        /// 代理请求处理器
        /// </summary>
        private class HttpRequestHandler : IHttpRequestLineHandler, IHttpHeadersHandler, IProxyFeature
        {
            private HttpMethod method;

            public HostString ProxyHost { get; private set; }

            public ProxyProtocol ProxyProtocol
            {
                get
                {
                    if (this.ProxyHost.HasValue == false)
                    {
                        return ProxyProtocol.None;
                    }
                    if (this.method == HttpMethod.Connect)
                    {
                        return ProxyProtocol.TunnelProxy;
                    }
                    return ProxyProtocol.HttpProxy;
                }
            }

            void IHttpRequestLineHandler.OnStartLine(HttpVersionAndMethod versionAndMethod, TargetOffsetPathLength targetPath, Span<byte> startLine)
            {
                this.method = versionAndMethod.Method;
                var host = Encoding.ASCII.GetString(startLine.Slice(targetPath.Offset, targetPath.Length));
                if (versionAndMethod.Method == HttpMethod.Connect)
                {
                    this.ProxyHost = HostString.FromUriComponent(host);
                }
                else if (Uri.TryCreate(host, UriKind.Absolute, out var uri))
                {
                    this.ProxyHost = HostString.FromUriComponent(uri);
                }
            }

            void IHttpHeadersHandler.OnHeader(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
            {
                // 这里可以处理认证头
                // 用于验证账号和密码
            }
            void IHttpHeadersHandler.OnHeadersComplete(bool endStream)
            {
            }
            void IHttpHeadersHandler.OnStaticIndexedHeader(int index)
            {
            }
            void IHttpHeadersHandler.OnStaticIndexedHeader(int index, ReadOnlySpan<byte> value)
            {
            }
        }
    }
}