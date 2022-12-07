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

        /// <summary>
        /// 收到Telnet连接后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnConnectedAsync(ConnectionContext context)
        {
            var input = context.Transport.Input;
            var output = context.Transport.Output;

            output.WriteLine($"Welcome to {Dns.GetHostName()} !");
            output.WriteLine($"It is {DateTime.Now} now !");
            await output.FlushAsync();

            while (context.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                if (TryReadMessage(result, out var message, out var consumed))
                {
                    await ProcessMessageAsync(context, message);
                    input.AdvanceTo(consumed);
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

        private static async ValueTask ProcessMessageAsync(ConnectionContext context, string message)
        {
            var output = context.Transport.Output;
            if (string.IsNullOrEmpty(message))
            {
                await output.WriteLineAsync("Please type something.");
            }
            else if (message.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                await output.WriteLineAsync("Have a good day!");
                context.Abort();
            }
            else
            {
                await output.WriteLineAsync($"Did you say '{message}'?");
            }
        }

        private static bool TryReadMessage(ReadResult result, out string message, out SequencePosition consumed)
        {
            var reader = new SequenceReader<byte>(result.Buffer);
            if (reader.TryReadTo(out ReadOnlySpan<byte> span, crlf))
            {
                message = Encoding.UTF8.GetString(span);
                consumed = reader.Position;
                return true;
            }
            else
            {
                message = string.Empty;
                consumed = result.Buffer.Start;
                return false;
            }
        }
    }
}