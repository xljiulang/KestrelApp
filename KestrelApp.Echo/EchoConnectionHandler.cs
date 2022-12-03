using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Echo
{
    sealed class EchoConnectionHandler : ConnectionHandler
    {
        private static readonly byte[] delimiters = Encoding.ASCII.GetBytes("\r\n");
        private readonly ILogger<EchoConnectionHandler> logger;

        public EchoConnectionHandler(ILogger<EchoConnectionHandler> logger)
        {
            this.logger = logger;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var client = connection.RemoteEndPoint;
            while (connection.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await connection.Transport.Input.ReadAsync();
                if (TryRead(result, out var echo, out var position))
                {
                    this.logger.LogInformation($"{client}: {echo}");

                    var respone = Encoding.UTF8.GetBytes($"{echo} ok");
                    await connection.Transport.Output.WriteAsync(respone);
                    await connection.Transport.Output.WriteAsync(delimiters);

                    connection.Transport.Input.AdvanceTo(position);
                }
                else
                {
                    var consumed = result.Buffer.Start;
                    connection.Transport.Input.AdvanceTo(consumed, position);
                }
            }
        }

        private static bool TryRead(ReadResult result, out string echo, out SequencePosition position)
        {
            var reader = new SequenceReader<byte>(result.Buffer);
            if (reader.TryReadTo(out ReadOnlySpan<byte> span, delimiters))
            {
                echo = Encoding.UTF8.GetString(span);
                position = reader.Position;
                return true;
            }
            else
            {
                echo = string.Empty;
                position = result.Buffer.GetPosition(reader.Length);
                return false;
            }
        }
    }
}