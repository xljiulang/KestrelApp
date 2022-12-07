using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>
    /// IDuplexPipe封装为Stream
    /// </summary>
    public class DuplexPipeStream : Stream
    {
        private readonly PipeReader input;
        private readonly PipeWriter output;
        private readonly bool throwOnCancelled;
        private volatile bool cancelCalled;

        /// <summary>
        /// IDuplexPipe封装为Stream
        /// </summary>
        /// <param name="duplexPipe"></param>
        /// <param name="throwOnCancelled"></param>
        public DuplexPipeStream(IDuplexPipe duplexPipe, bool throwOnCancelled = false)
        {
            input = duplexPipe.Input;
            output = duplexPipe.Output;
            this.throwOnCancelled = throwOnCancelled;
        }

        /// <summary>
        /// 取消挂起的读取操作
        /// </summary>
        public void CancelPendingRead()
        {
            cancelCalled = true;
            input.CancelPendingRead();
        }

        /// <inheritdoc/>
        public override bool CanRead => true;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite => true;

        /// <inheritdoc/>
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var task = ReadAsyncInternal(new Memory<byte>(buffer, offset, count), default);
            return task.IsCompleted ? task.Result : task.AsTask().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            return ReadAsyncInternal(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
        {
            return ReadAsyncInternal(destination, cancellationToken);
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public override async Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await output.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);
        }

        /// <inheritdoc/>
        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
        {
            await output.WriteAsync(source, cancellationToken);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await output.FlushAsync(cancellationToken);
        }


        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        private async ValueTask<int> ReadAsyncInternal(Memory<byte> destination, CancellationToken cancellationToken)
        {
            while (true)
            {
                var result = await input.ReadAsync(cancellationToken);
                var readableBuffer = result.Buffer;
                try
                {
                    if (throwOnCancelled && result.IsCanceled && cancelCalled)
                    {
                        // Reset the bool
                        cancelCalled = false;
                        throw new OperationCanceledException();
                    }

                    if (!readableBuffer.IsEmpty)
                    {
                        // buffer.Count is int
                        var count = (int)Math.Min(readableBuffer.Length, destination.Length);
                        readableBuffer = readableBuffer.Slice(0, count);
                        readableBuffer.CopyTo(destination.Span);
                        return count;
                    }

                    if (result.IsCompleted)
                    {
                        return 0;
                    }
                }
                finally
                {
                    input.AdvanceTo(readableBuffer.End, readableBuffer.End);
                }
            }
        }

        /// <inheritdoc/>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return TaskToApm.Begin(ReadAsync(buffer, offset, count), callback, state);
        }

        /// <inheritdoc/>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return TaskToApm.End<int>(asyncResult);
        }

        /// <inheritdoc/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
        {
            return TaskToApm.Begin(WriteAsync(buffer, offset, count), callback, state);
        }

        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            TaskToApm.End(asyncResult);
        }
    }
}
