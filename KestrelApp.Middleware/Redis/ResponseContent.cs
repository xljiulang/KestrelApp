using System;
using System.Text;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 响应内容
    /// </summary>
    abstract class ResponseContent
    {
        /// <summary>
        /// OK
        /// </summary>
        public static ResponseContent OK { get; } = new StringContent("+OK\r\n");

        /// <summary>
        /// Error
        /// </summary>
        public static ResponseContent Err { get; } = new StringContent("-ERR\r\n");

        /// <summary>
        /// pong
        /// </summary>
        public static ResponseContent Pong { get; } = new StringContent("+PONG\r\n");


        public abstract ReadOnlyMemory<byte> ToMemory();

        /// <summary>
        /// 文本响应内容
        /// </summary>
        private class StringContent : ResponseContent
        {
            private readonly ReadOnlyMemory<byte> value;

            public StringContent(string value)
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
