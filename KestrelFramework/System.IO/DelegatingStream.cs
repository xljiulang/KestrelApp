using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>
    /// 委托流
    /// </summary>
    public abstract class DelegatingStream : Stream
    {
        protected Stream Inner { get; }

        /// <summary>
        /// 委托流
        /// </summary>
        /// <param name="inner"></param>
        public DelegatingStream(Stream inner)
        {
            Inner = inner;
        }

        public override bool CanRead
        {
            get
            {
                return Inner.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return Inner.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return Inner.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return Inner.Length;
            }
        }

        public override long Position
        {
            get
            {
                return Inner.Position;
            }

            set
            {
                Inner.Position = value;
            }
        }

        public override void Flush()
        {
            Inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Inner.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Inner.Read(buffer, offset, count);
        }

        public override int Read(Span<byte> destination)
        {
            return Inner.Read(destination);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Inner.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
        {
            return Inner.ReadAsync(destination, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            Inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Inner.Write(buffer, offset, count);
        }

        public override void Write(ReadOnlySpan<byte> source)
        {
            Inner.Write(source);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Inner.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
        {
            return Inner.WriteAsync(source, cancellationToken);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return TaskToApm.Begin(ReadAsync(buffer, offset, count), callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return TaskToApm.End<int>(asyncResult);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return TaskToApm.Begin(WriteAsync(buffer, offset, count), callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            TaskToApm.End(asyncResult);
        }
    }
}
