using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KestrelApp.Transforms.Security
{
    sealed class SecurityStream : DelegatingStream
    {
        public SecurityStream(Stream inner)
            : base(inner)
        {
        }

        public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
        {
            // 先读取再解密流量
            return base.ReadAsync(destination, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
        {
            // 先加密再写入
            return base.WriteAsync(source, cancellationToken);
        }
    }
}
