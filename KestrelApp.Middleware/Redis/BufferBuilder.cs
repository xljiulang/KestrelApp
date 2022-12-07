using System;
using System.Buffers;
using System.Text;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示buffer构建器
    /// </summary>
    sealed class BufferBuilder
    {
        private readonly ArrayBufferWriter<byte> writer = new(256);

        public ReadOnlyMemory<byte> WrittenMemory => this.writer.WrittenMemory;

        /// <summary>
        /// 写入\r\n
        /// </summary>
        /// <returns></returns>
        public BufferBuilder WriteLine()
        {
            this.writer.WriteCRLF();
            return this;
        }

        public BufferBuilder Write(ReadOnlyMemory<byte> value)
        {
            this.writer.Write(value.Span);
            return this;
        }

        public BufferBuilder Write(string value)
        {
            this.writer.Write(value, Encoding.ASCII);
            return this;
        }


        public BufferBuilder Write(char value)
        {
            this.writer.Write((byte)value);
            return this;
        } 
    }
}
