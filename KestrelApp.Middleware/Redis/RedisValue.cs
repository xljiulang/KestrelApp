using System;
using System.Text;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis值
    /// </summary>
    sealed class RedisValue
    {
        /// <summary>
        /// 获取值
        /// </summary>
        public ReadOnlyMemory<byte> Value { get; }

        /// <summary>
        /// Redis值
        /// </summary>
        /// <param name="value"></param>
        public RedisValue(ReadOnlyMemory<byte> value)
        {
            this.Value = value;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.Value.Span);
        }
    }
}
