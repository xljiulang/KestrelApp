using Microsoft.AspNetCore.Connections;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Telnet
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
        /// <param name="connection"></param>
        /// <returns></returns>
        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var input = connection.Transport.Input;
            var output = connection.Transport.Output;

            output.WriteLine($"Welcome to {Dns.GetHostName()} !");
            output.WriteLine($"It is {DateTime.Now} now !");
            await output.FlushAsync(connection.ConnectionClosed);

            while (connection.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                if (TryReadMessage(result, out var message, out var consumed))
                {
                    await ProcessMessageAsync(message, connection);
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

        private static async ValueTask ProcessMessageAsync(string message, ConnectionContext connection)
        {
            var output = connection.Transport.Output;
            if (string.IsNullOrEmpty(message))
            {
                await output.WriteLineAsync("Please type something.");
            }
            else if (message.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                await output.WriteLineAsync("Have a good day!");
                connection.Abort();
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