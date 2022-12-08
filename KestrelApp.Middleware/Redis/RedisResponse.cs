using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示redis回复
    /// </summary>
    sealed class RedisResponse
    {
        private readonly PipeWriter writer;

        public RedisResponse(PipeWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// 写入\r\n
        /// </summary>
        /// <returns></returns>
        public RedisResponse WriteLine()
        {
            this.writer.WriteCRLF();
            return this;
        }

        public RedisResponse Write(char value)
        {
            this.writer.Write((byte)value);
            return this;
        }

        public RedisResponse Write(ReadOnlySpan<char> value)
        {
            this.writer.Write(value, Encoding.UTF8);
            return this;
        }

        public RedisResponse Write(ReadOnlyMemory<byte> value)
        {
            this.writer.Write(value.Span);
            return this;
        }


        public ValueTask<FlushResult> FlushAsync()
        {
            return this.writer.FlushAsync();
        }

        public ValueTask<FlushResult> WriteAsync(ResponseContent content)
        { 
            return this.writer.WriteAsync(content.ToMemory());
        }
    }
}
