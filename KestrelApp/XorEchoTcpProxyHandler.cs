using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace KestrelApp.Transforms.SecurityProxy
{
    // 这里可以使用Socket直接连接到目标服务器
    // 也可以使用IConnectionFactory来创建抽象连接
    sealed class XorEchoTcpProxyHandler : ConnectionHandler
    {
        private readonly ILogger<XorEchoTcpProxyHandler> logger;
        private readonly IConnectionFactory connectionFactory;

        // 代理到 XorEcho 服务器
        private readonly IPEndPoint xorEchoServer = new(IPAddress.Loopback, 5001);

        /// <summary>
        /// 将流量代理到XorEcho服务器
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionFactory"></param>
        public XorEchoTcpProxyHandler(
            ILogger<XorEchoTcpProxyHandler> logger,
            IConnectionFactory connectionFactory)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            this.logger.LogInformation($"{connection.RemoteEndPoint}-->{connection.LocalEndPoint}-->{xorEchoServer}");
            var upstream = await this.connectionFactory.ConnectAsync(xorEchoServer);
            var task1 = connection.Transport.Input.CopyToAsync(upstream.Transport.Output);
            var task2 = upstream.Transport.Input.CopyToAsync(connection.Transport.Output);
            await Task.WhenAny(task1, task2);
        }
    }
}
