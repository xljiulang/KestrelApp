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
        private static readonly byte[] delimiters = Encoding.ASCII.GetBytes("\r\n");

        /// <summary>
        /// 收到Telnet连接后
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var input = connection.Transport.Input;
            var output = connection.Transport.Output;

            output.Write($"Welcome to {Dns.GetHostName()} !\r\n");
            output.Write($"It is {DateTime.Now} now !\r\n");
            await output.FlushAsync(connection.ConnectionClosed);

            while (connection.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (TryReadMessage(result, out var message, out var position))
                {
                    await ProcessMessageAsync(message, connection);
                    input.AdvanceTo(position);
                }
                else
                {
                    var consumed = result.Buffer.Start;
                    input.AdvanceTo(consumed, position);
                }
            }
        }

        private static async ValueTask ProcessMessageAsync(string message, ConnectionContext connection)
        {
            var output = connection.Transport.Output;
            if (string.IsNullOrEmpty(message))
            {
                await output.WriteAsync("Please type something.\r\n");
            }
            else if (message.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                await output.WriteAsync("Have a good day!\r\n");
                connection.Abort();
            }
            else
            {
                await output.WriteAsync($"Did you say '{message}'?\r\n");
            }
        }

        private static bool TryReadMessage(ReadResult result, out string message, out SequencePosition position)
        {
            var reader = new SequenceReader<byte>(result.Buffer);
            if (reader.TryReadTo(out ReadOnlySpan<byte> span, delimiters))
            {
                message = Encoding.UTF8.GetString(span);
                position = reader.Position;
                return true;
            }
            else
            {
                message = string.Empty;
                position = result.Buffer.GetPosition(reader.Length);
                return false;
            }
        }
    }
}