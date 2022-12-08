using KestrelApp.Middleware.Telnet.Middleware;
using KestrelFramework.Application;
using Microsoft.AspNetCore.Connections;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Telnet
{
    /// <summary>
    /// Telnet连接协议处理
    /// </summary>
    public class TelnetConnectionHandler : ConnectionHandler
    {
        private static readonly byte[] crlf = Encoding.ASCII.GetBytes("\r\n");

        private readonly ApplicationDelegate<TelnetContext> application;

        /// <summary>
        /// Telnet连接协议处理
        /// </summary>
        /// <param name="appServices"></param>
        public TelnetConnectionHandler(IServiceProvider appServices)
        {
            this.application = new ApplicationBuilder<TelnetContext>(appServices)
                .Use<EmptyMiddleware>()
                .Use<ByeMiddlware>()
                .Use<EchoMiddleware>()
                .Build();
        }

        /// <summary>
        /// 收到Telnet连接后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnConnectedAsync(ConnectionContext context)
        {
            var input = context.Transport.Input;
            var output = context.Transport.Output;

            var response = new TelnetResponse(output);
            context.Features.Set(response);

            response.WriteLine($"Welcome to {Dns.GetHostName()} !");
            response.WriteLine($"It is {DateTime.Now} now !");
            await response.FlushAsync();


            while (context.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                if (TryReadRequest(result, out var request, out var consumed))
                {
                    input.AdvanceTo(consumed);

                    var telnetContext = new TelnetContext(request, context);
                    await this.application.Invoke(telnetContext);
                }
                else
                {
                    input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }

        private static bool TryReadRequest(ReadResult result, out string request, out SequencePosition consumed)
        {
            var reader = new SequenceReader<byte>(result.Buffer);
            if (reader.TryReadTo(out ReadOnlySpan<byte> span, crlf))
            {
                request = Encoding.UTF8.GetString(span);
                consumed = reader.Position;
                return true;
            }
            else
            {
                request = string.Empty;
                consumed = result.Buffer.Start;
                return false;
            }
        }
    }
}