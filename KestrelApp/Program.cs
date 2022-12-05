using KestrelApp.Telnet;
using KestrelApp.HttpProxy;
using KestrelApp.Transforms.SecurityProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace KestrelApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddConnections()
                .AddHttproxy()
                .AddFlowAnalyze()
                .AddTlsDetect()
                .AddConnectionFactory();

            builder.Host.UseSerilog((hosting, logger) =>
            {
                logger.ReadFrom
                    .Configuration(hosting.Configuration)
                    .Enrich.FromLogContext().WriteTo.Console(outputTemplate: "{Timestamp:O} [{Level:u3}]{NewLine}{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
            });

            builder.WebHost.ConfigureKestrel((context, kestrel) =>
            {
                var section = context.Configuration.GetSection("Kestrel");
                kestrel.Configure(section)
                    // 普通Telnet服务器,使用telnet客户端就可以交互
                    .Endpoint("Telnet", endpoint => endpoint.ListenOptions.UseEcho())

                    // xor(伪)加密传输的Telnet服务器, telnet客户端不能交互
                    .Endpoint("XorTelnet", endpoint => endpoint.ListenOptions.UseFlowXor().UseEcho())

                    // XorTelnet代理服务器，telnet连接到此服务器之后，它将流量xor之后代理到XorTelnet服务器，它本身不参与Telnet协议处理
                    .Endpoint("XorTelnetProxy", endpoint => endpoint.ListenOptions.UseFlowXor().UseConnectionHandler<XorTelnetProxyHandler>())

                    // http代理服务器，能处理隧道代理的场景
                    .Endpoint("HttpProxy", endpoint => endpoint.ListenOptions.UseHttpProxy())

                    // http和https单端口双协议服务器
                    .Endpoint("HttpHttps", endpoint => endpoint.ListenOptions.UseTlsDetect(option => { }));
            });

            var app = builder.Build();
            app.UseRouting();

            // http代理中间件，能处理非隧道的http代理请求
            app.UseMiddleware<HttpProxyMiddleware>();

            // Telnet over WebSocket
            app.MapConnectionHandler<TelnetConnectionHandler>("/telnet");

            app.Map("/{**any}", async context => await context.Response.WriteAsync(nameof(KestrelApp)));
            app.Run();
        }
    }
}