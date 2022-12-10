using KestrelApp.Fiddler.Kestrel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace KestrelApp.Fiddler.Http
{
    /// <summary>
    /// http代理执行中间件
    /// </summary>
    sealed class HttpProxyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpForwarder httpForwarder;
        private readonly ILogger<HttpProxyMiddleware> logger;
        private readonly HttpMessageInvoker httpClient = new(CreateSocketsHttpHandler());

        /// <summary>
        /// http代理执行中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="httpForwarder"></param>
        /// <param name="logger"></param>
        public HttpProxyMiddleware(
            RequestDelegate next,
            IHttpForwarder httpForwarder,
            ILogger<HttpProxyMiddleware> logger)
        {
            this.next = next;
            this.httpForwarder = httpForwarder;
            this.logger = logger;
        }

        /// <summary>
        /// 转发http流量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature == null || feature.ProxyProtocol == ProxyProtocol.None)
            {
                await next(context);
            }
            else
            {
                var scheme = context.Request.Scheme;
                var destinationPrefix = $"{scheme}://{feature.ProxyHost}";
                await httpForwarder.SendAsync(context, destinationPrefix, httpClient, ForwarderRequestConfig.Empty, HttpTransformer.Empty);
            }
        }

        private static SocketsHttpHandler CreateSocketsHttpHandler()
        {
            return new SocketsHttpHandler
            {
                Proxy = null,
                UseProxy = false,
                UseCookies = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.None,
            };
        }
    }
}
