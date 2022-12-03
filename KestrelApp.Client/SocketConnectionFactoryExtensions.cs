using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KestrelApp
{
    public static class SocketConnectionFactoryExtensions
    {
        /// <summary>
        /// 注册SocketConnectionFactory为IConnectionFactory
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSocketConnectionFactory(this IServiceCollection services)
        {
            const string typeName = "Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.SocketConnectionFactory";
            var factoryType = typeof(SocketTransportOptions).Assembly.GetType(typeName);
            return factoryType == null
                ? throw new NotSupportedException($"找不到类型{typeName}")
                : services.AddSingleton(typeof(IConnectionFactory), factoryType);
        }
    }
}