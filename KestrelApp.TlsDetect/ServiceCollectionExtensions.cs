using KestrelApp.TlsDetect;
using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Tls侦测中间件
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddTlsDetect(this IServiceCollection services)
        { 
            return services
                .AddSingleton<TlsInvadeMiddleware>()
                .AddSingleton<TlsRestoreMiddleware>();
        }
    }
}
