using System;
using System.Buffers;
using System.Text;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示redis回复
    /// </summary>
    class RedisResponse : IRedisResponse
    {
        /// <summary>
        /// OK
        /// </summary>
        public static IRedisResponse OK { get; } = new StringResponse("+OK\r\n");

        /// <summary>
        /// Error
        /// </summary>
        public static IRedisResponse Err { get; } = new StringResponse("-ERR\r\n");

        /// <summary>
        /// pong
        /// </summary>
        public static IRedisResponse Pong { get; } = new StringResponse("+PONG\r\n");


        private readonly ArrayBufferWriter<byte> writer = new();

        /// <summary>
        /// 写入\r\n
        /// </summary>
        /// <returns></returns>
        public RedisResponse WriteLine()
        {
            this.writer.WriteCRLF();
            return this;
        }

        public RedisResponse Write(ReadOnlyMemory<byte> value)
        {
            this.writer.Write(value.Span);
            return this;
        }

        public RedisResponse Write(ReadOnlySpan<char> value)
        {
            this.writer.Write(value, Encoding.UTF8);
            return this;
        }

        public RedisResponse Write(char value)
        {
            this.writer.Write((byte)value);
            return this;
        }

        /// <summary>
        /// 转换为Memory
        /// </summary>
        /// <returns></returns>
        public ReadOnlyMemory<byte> ToMemory()
        {
            return this.writer.WrittenMemory;
        }

        /// <summary>
        /// 文本回复
        /// </summary>
        private class StringResponse : IRedisResponse
        {
            private readonly ReadOnlyMemory<byte> value;

            public StringResponse(string value)
            {
                this.value = Encoding.UTF8.GetBytes(value);
            }

            public ReadOnlyMemory<byte> ToMemory()
            {
                return this.value;
            }
        }
    }
}
