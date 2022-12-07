using System;
using System.Text;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示redis回复
    /// </summary>
    abstract class RedisResponse
    {
        /// <summary>
        /// OK
        /// </summary>
        public static RedisResponse OK { get; } = new StringRepy("+OK\r\n");

        /// <summary>
        /// Error
        /// </summary>
        public static RedisResponse Err { get; } = new StringRepy("-ERR\r\n");

        /// <summary>
        /// pong
        /// </summary>
        public static RedisResponse Pong { get; } = new StringRepy("+PONG\r\n");

        /// <summary>
        /// 转换为Memory
        /// </summary>
        /// <returns></returns>
        public abstract ReadOnlyMemory<byte> ToMemory();

        /// <summary>
        /// 数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static RedisResponse Number(int value)
        {
            return new NumberReply(value);
        }
        
        /// <summary>
        /// 数字回复
        /// </summary>
        private class NumberReply : StringRepy
        {
            public NumberReply(int value)
                : base($":{value}\r\n")
            {
            }
        }

        /// <summary>
        /// 文本回复
        /// </summary>
        private class StringRepy : RedisResponse
        {
            private readonly Memory<byte> value;

            public StringRepy(string value)
            {
                this.value = Encoding.UTF8.GetBytes(value);
            }

            public override ReadOnlyMemory<byte> ToMemory()
            {
                return this.value;
            }
        }
    }
}
