using KestrelApp.Middleware.Redis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param> 
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services, Action<RedisOptions> configureOptions)
        {
            return services.AddRedis().Configure(configureOptions);
        }

        /// <summary>
        /// 添加Redis
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            var serviceType = typeof(IRedisCmdHanler);
            var cmdHandlerTypes = serviceType.Assembly.GetTypes()
                .Where(item => serviceType.IsAssignableFrom(item))
                .Where(item => item.IsClass && item.IsAbstract == false);

            foreach (var implementationType in cmdHandlerTypes)
            {
                var descriptor = ServiceDescriptor.Singleton(serviceType, implementationType);
                services.TryAddEnumerable(descriptor);
            }

            return services;
        }
    }
}
