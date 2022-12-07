using Microsoft.AspNetCore.Connections;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册SocketConnectionFactory为IConnectionFactory
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSocketConnectionFactory(this IServiceCollection services)
        {
            var factoryType = ConnectionFactoryTypeUtil.FindSocketConnectionFactory();
            return services.AddSingleton(typeof(IConnectionFactory), factoryType);
        }
    }
}