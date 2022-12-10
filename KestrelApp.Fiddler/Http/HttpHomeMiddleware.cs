using KestrelApp.Fiddler.Kestrel;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler.Http
{
    /// <summary>
    /// http主页中间件
    /// </summary>
    sealed class HttpHomeMiddleware
    {
        private readonly RequestDelegate next;

        public HttpHomeMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// 处理http主页
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature == null)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    Error = "找不到IProxyFeature"
                });
            }
            else if (feature.ProxyProtocol == ProxyProtocol.None)
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "欢迎使用Fiddler，请在远程客户端设备上安装Fiddler的跟证书"
                });
            }
            else
            {
                await next(context);
            }
        }
    }
}
