using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Yarp.ReverseProxy.Forwarder;

namespace KestrelApp.Fiddler.Middlewares
{
    /// <summary>
    /// http代理执行中间件
    /// </summary>
    sealed class HttpForwardMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHttpForwarder httpForwarder;
        private readonly HttpMessageInvoker httpClient = new(CreateSocketsHttpHandler());

        /// <summary>
        /// http代理执行中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="httpForwarder"></param>
        public HttpForwardMiddleware(
            RequestDelegate next,
            IHttpForwarder httpForwarder)
        {
            this.next = next;
            this.httpForwarder = httpForwarder;
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
