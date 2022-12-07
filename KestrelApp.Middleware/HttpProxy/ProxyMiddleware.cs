using KestrelFramework;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.HttpProxy
{
    /// <summary>
    /// 代理中间件
    /// </summary>
    sealed class ProxyMiddleware : IKestrelMiddleware
    {
        private static readonly byte[] http400 = Encoding.ASCII.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n");
        private static readonly byte[] http407 = Encoding.ASCII.GetBytes("HTTP/1.1 407 Proxy Authentication Required\r\n\r\n");

        private readonly HttpParser<HttpRequestHandler> httpParser = new();
        private readonly IHttpProxyAuthenticationHandler authenticationHandler;

        /// <summary>
        /// 代理中间件
        /// </summary>
        /// <param name="authenticationHandler"></param>
        public ProxyMiddleware(IHttpProxyAuthenticationHandler authenticationHandler)
        {
            this.authenticationHandler = authenticationHandler;
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
                        if (request.ProxyProtocol == ProxyProtocol.TunnelProxy)
                        {
                            input.AdvanceTo(consumed);
                        }
                        else
                        {
                            input.AdvanceTo(result.Buffer.Start);
                        }

                        // 身份认证
                        if (await this.authenticationHandler.AuthenticateAsync(request.ProxyAuthorization))
                        {
                            context.Features.Set<IProxyFeature>(request);
                            await next(context);
                        }
                        else
                        {
                            await output.WriteAsync(http407);
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
                    await output.WriteAsync(http400);
                    break;
                }
            }
        }


        /// <summary>
        /// 解析http请求
        /// </summary>
        /// <param name="result"></param>
        /// <param name="request"></param>
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

            public AuthenticationHeaderValue? ProxyAuthorization { get; private set; }


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
                const string proxyAuthorization = "Proxy-Authorization";
                var headerName = Encoding.ASCII.GetString(name);
                if (proxyAuthorization.Equals(headerName, StringComparison.OrdinalIgnoreCase))
                {
                    var headerValue = Encoding.ASCII.GetString(value);
                    if (AuthenticationHeaderValue.TryParse(headerValue, out var authentication))
                    {
                        this.ProxyAuthorization = authentication;
                    }
                }
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