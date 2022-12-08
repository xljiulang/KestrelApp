using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Echo
{
    /// <summary>
    /// Echo协议连接协议处理者
    /// </summary>
    public class EchoConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<EchoConnectionHandler> logger;
        private static readonly byte[] helloWorld = Encoding.UTF8.GetBytes("Hello world");

        /// <summary>
        /// Echo协议连接协议处理者
        /// </summary>
        /// <param name="logger"></param>
        public EchoConnectionHandler(ILogger<EchoConnectionHandler> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 收到Echo连接后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnConnectedAsync(ConnectionContext context)
        {
            var input = context.Transport.Input;
            var output = context.Transport.Output;

            output.WriteBigEndian((ushort)helloWorld.Length);
            output.Write(helloWorld);
            await output.FlushAsync();

            while (context.ConnectionClosed.IsCancellationRequested == false)
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
                    reader.UnreadSequence.Slice(0, length).CopyTo(echo.Array);
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