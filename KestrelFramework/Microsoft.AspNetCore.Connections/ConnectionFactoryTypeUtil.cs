using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using System;

namespace Microsoft.AspNetCore.Connections
{
    static class ConnectionFactoryTypeUtil
    {
        private const string socketConnectionFactoryTypeName = "Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.SocketConnectionFactory";

        /// <summary>
        /// 查找SocketConnectionFactory的类型
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Type FindSocketConnectionFactory()
        {
            var assembly = typeof(SocketTransportOptions).Assembly;
            var connectionFactoryType = assembly.GetType(socketConnectionFactoryTypeName);
            return connectionFactoryType ?? throw new NotSupportedException($"找不到类型{socketConnectionFactoryTypeName}");
        }
    }
}
