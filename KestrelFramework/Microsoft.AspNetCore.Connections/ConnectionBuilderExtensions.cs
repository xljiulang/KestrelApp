using KestrelFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Connections
{
    /// <summary>
    /// IConnectionBuilder扩展
    /// </summary>
    public static class ConnectionBuilderExtensions
    {
        /// <summary>
        /// 使用Kestrel中间件
        /// </summary>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IConnectionBuilder Use<TMiddleware>(this IConnectionBuilder builder)
            where TMiddleware : IKestrelMiddleware
        {
            var middleware = ActivatorUtilities.GetServiceOrCreateInstance<TMiddleware>(builder.ApplicationServices);
            return builder.Use(middleware);
        }

        /// <summary>
        /// 使用Kestrel中间件
        /// </summary> 
        /// <param name="builder"></param>
        /// <param name="middleware"></param>
        /// <returns></returns>
        public static IConnectionBuilder Use(this IConnectionBuilder builder, IKestrelMiddleware middleware)
        {
            return builder.Use(next => context => middleware.InvokeAsync(next, context));
        }
    }
}
