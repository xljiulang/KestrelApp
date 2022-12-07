using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using System;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KestrelApp.HttpProxy
{
    /// <summary>
    /// 隧道代理连接
    /// </summary>
    sealed class TunnelProxyConnection : IDisposable
    {
        private readonly IProxyFeature feature;
        private readonly Socket socket;

        public TunnelProxyConnection(IProxyFeature feature)
        {
            this.feature = feature;
            this.socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 创建远程端的连接
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            var port = this.feature.ProxyHost.Port;
            if (port == null)
            {
                throw new InvalidOperationException($"找不到远程连接端口");
            }

            var host = this.feature.ProxyHost.Host;
            await this.socket.ConnectAsync(host, port.Value, cancellationToken);
        }

        /// <summary>
        /// 处理传输
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task TransportAsync(ConnectionContext context)
        {
            var transport = context.Features.Get<IConnectionTransportFeature>()?.Transport;
            if (transport == null)
            {
                throw new InvalidOperationException($"找不到IConnectionTransportFeature");
            }

            var cancellationToken = context.ConnectionClosed;
            var stream = new NetworkStream(this.socket, ownsSocket: false);

            var task1 = stream.CopyToAsync(transport.Output, cancellationToken);
            var task2 = transport.Input.CopyToAsync(stream, cancellationToken);
            await Task.WhenAny(task1, task2);
        }

        public void Dispose()
        {
            this.socket.Dispose();
        }
    }
}
