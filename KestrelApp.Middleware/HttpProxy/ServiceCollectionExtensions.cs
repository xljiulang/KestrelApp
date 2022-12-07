using KestrelApp.Middleware.HttpProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加http代理
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddHttproxy(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpProxyAuthenticationHandler, HttpProxyAuthenticationHandler>();
            return services
                .AddSingleton<ProxyMiddleware>()
                .AddSingleton<TunnelProxyMiddleware>()
                .AddHttpForwarder();
        }
    }
}
