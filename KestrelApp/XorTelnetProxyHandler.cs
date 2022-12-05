using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace KestrelApp.Transforms.SecurityProxy
{
    // 这里可以使用Socket直接连接到目标服务器
    // 也可以使用IConnectionFactory来创建抽象连接
    sealed class XorTelnetProxyHandler : ConnectionHandler
    {
        private readonly ILogger<XorTelnetProxyHandler> logger;
        private readonly IConnectionFactory connectionFactory;

        // 代理到 XorTelnet 服务器
        private readonly IPEndPoint xorTelnetServer = new(IPAddress.Loopback, 5001);

        /// <summary>
        /// 将流量代理到XorTelnet服务器
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionFactory"></param>
        public XorTelnetProxyHandler(
            ILogger<XorTelnetProxyHandler> logger,
            IConnectionFactory connectionFactory)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            this.logger.LogInformation($"{connection.RemoteEndPoint}-->{connection.LocalEndPoint}-->{xorTelnetServer}");
            var upstream = await this.connectionFactory.ConnectAsync(xorTelnetServer);
            var task1 = connection.Transport.Input.CopyToAsync(upstream.Transport.Output);
            var task2 = upstream.Transport.Input.CopyToAsync(connection.Transport.Output);
            await Task.WhenAny(task1, task2);
        }
    }
}
