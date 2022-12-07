using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.FlowXor
{
    /// <summary>
    /// 对数据进行异或（加密/解密模拟）处理
    /// </summary>
    sealed class XorStream : DelegatingStream
    {
        private const byte xorByte = 9;
        private readonly ILogger logger;

        public XorStream(Stream inner, ILogger logger)
            : base(inner)
        {
            this.logger = logger;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
        {
            var length = await base.ReadAsync(destination, cancellationToken);
            this.logger.LogInformation($"xor还原了{length}字节");
            Xor(destination.Span);
            return length;
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
        {
            using var owner = ArrayPool<byte>.Shared.RentArrayOwner(source.Length);
            var memory = owner.AsMemory();
            this.logger.LogInformation($"xor混淆了{memory.Length}字节");

            source.CopyTo(memory);
            Xor(memory.Span);
            await base.WriteAsync(memory, cancellationToken);
        }


        private static void Xor(Span<byte> span)
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = (byte)(span[i] ^ xorByte);
            }
        }
    }
}
