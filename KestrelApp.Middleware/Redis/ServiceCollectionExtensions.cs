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
            var handlerTypes = typeof(ICmdHanler).Assembly.GetTypes()
                .Where(item => typeof(ICmdHanler).IsAssignableFrom(item))
                .Where(item => item.IsAbstract == false);

            foreach (var type in handlerTypes)
            {
                var descriptor = ServiceDescriptor.Singleton(typeof(ICmdHanler), type);
                services.TryAddEnumerable(descriptor);
            }

            return services;
        }
    }
}
