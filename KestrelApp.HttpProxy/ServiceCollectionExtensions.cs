using KestrelApp.HttpProxy;
using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加http代理
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddHttproxy(this IServiceCollection services)
        {
            return services
                .AddSingleton<ProxyMiddleware>()
                .AddHttpForwarder();
        }
    }
}
