using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Echo
{
    /// <summary>
    /// Echo协议连接协议处理
    /// </summary>
    public class EchoConnectionHandler : ConnectionHandler
    {
        private static readonly byte[] hello = Encoding.UTF8.GetBytes("Hello world");
        private readonly ILogger<EchoConnectionHandler> logger;

        public EchoConnectionHandler(ILogger<EchoConnectionHandler> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 收到Echo连接后
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            var input = connection.Transport.Input;
            var output = connection.Transport.Output;

            output.WriteBigEndian((ushort)hello.Length);
            output.Write(hello);

            while (connection.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                if (TryReadEcho(result, out var echo, out var consumed))
                {
                    using (echo)
                    {
                        var text = Encoding.UTF8.GetString(echo.Array, 0, echo.Length);
                        this.logger.LogInformation($"Received from server: {text}");

                        output.WriteBigEndian((ushort)echo.Length);
                        output.Write(echo.Array.AsSpan(0, echo.Length));
                        await output.FlushAsync();
                    }

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

        private static bool TryReadEcho(ReadResult result, [MaybeNullWhen(false)] out IArrayOwner<byte> echo, out SequencePosition consumed)
        {
            var reader = new SequenceReader<byte>(result.Buffer);
            if (reader.TryReadBigEndian(out short length))
            {
                if (reader.Remaining >= length)
                {
                    echo = ArrayPool<byte>.Shared.RentArrayOwner(length);
                    reader.UnreadSpan[..length].CopyTo(echo.Array);
                    reader.Advance(length);

                    consumed = reader.Position;
                    return true;
                }
            }

            echo = null;
            consumed = result.Buffer.Start;
            return false;
        }
    }
}