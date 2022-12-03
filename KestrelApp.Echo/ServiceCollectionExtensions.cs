using KestrelApp.Echo;
using Microsoft.Extensions.DependencyInjection;

namespace KestrelApp
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Echo服务
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddEcho(this IServiceCollection services)
        {
            return services.AddSingleton<EchoConnectionHandler>();
        }
    }
}
