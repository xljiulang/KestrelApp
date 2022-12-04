using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KestrelApp.Transforms.Security
{
    /// <summary>
    /// 对数据进行异或（加密/解密模拟）处理
    /// </summary>
    sealed class XorStream : DelegatingStream
    {
        private const byte xorByte = 9;

        public XorStream(Stream inner)
            : base(inner)
        {
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
        {
            var length = await base.ReadAsync(destination, cancellationToken);
            Xor(destination.Span);
            return length;

        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(source.Length);
            try
            {
                var memory = buffer.AsMemory(0, source.Length);
                source.CopyTo(memory);
                Xor(memory.Span);
                await base.WriteAsync(memory, cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
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
