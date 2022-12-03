using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace KestrelApp.HttpProxy
{
    /// <summary>
    /// 普通http代理(非隧道代理)中间件
    /// </summary>
    public class HttpProxyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpForwarder httpForwarder;
        private readonly ILogger<HttpProxyMiddleware> logger;
        private readonly HttpMessageInvoker httpClient = new(CreateSocketsHttpHandler());

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
        /// 处理http代理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature == null || feature.ProxyProtocol != ProxyProtocol.HttpProxy)
            {
                await next(context);
            }
            else
            {
                var scheme = context.Request.Scheme;
                var destinationPrefix = $"{scheme}://{feature.ProxyHost}";
                this.logger.LogInformation($"http代理到{destinationPrefix}");
                await httpForwarder.SendAsync(context, destinationPrefix, this.httpClient, ForwarderRequestConfig.Empty, HttpTransformer.Empty);
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
