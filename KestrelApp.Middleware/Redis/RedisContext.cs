using KestrelFramework.Application;
using Microsoft.AspNetCore.Connections;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示redis上下文
    /// </summary>
    sealed class RedisContext : ApplicationContext
    {
        /// <summary>
        /// 获取redis客户端
        /// </summary>
        public RedisClient Client { get; }

        /// <summary>
        /// 获取redis命令
        /// </summary>
        public RedisCmd Reqeust { get; }

        /// <summary>
        /// redis上下文
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="context"></param>
        public RedisContext(RedisClient client, RedisCmd request, ConnectionContext context)
            : base(context.Features)
        {
            this.Client = client;
            this.Reqeust = request;
        }

        public override string ToString()
        {
            return $"{this.Client} {this.Reqeust}";
        }
    }
}
