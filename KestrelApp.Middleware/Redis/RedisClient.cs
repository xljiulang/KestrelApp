using Microsoft.AspNetCore.Connections;
using System.Net;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis客户端
    /// </summary>
    sealed class RedisClient
    {
        private readonly ConnectionContext context;

        /// <summary>
        /// 获取或设置是否已授权
        /// </summary>
        public bool? IsAuthed { get; set; }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public EndPoint? RemoteEndPoint => context.RemoteEndPoint;

        /// <summary>
        /// Redis客户端
        /// </summary>
        /// <param name="context"></param> 
        public RedisClient(ConnectionContext context)
        {
            this.context = context;
        }
        
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            this.context.Abort();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return this.RemoteEndPoint?.ToString();
        }
    }
}
