using KestrelFramework.Application;
using Microsoft.AspNetCore.Http.Features;

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
        /// 获取redis请求
        /// </summary>
        public RedisRequest Reqeust { get; }

        /// <summary>
        /// 获取redis响应
        /// </summary>
        public RedisResponse Response { get; }

        /// <summary>
        /// redis上下文
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="features"></param> 
        public RedisContext(RedisClient client, RedisRequest request, RedisResponse response, IFeatureCollection features)
            : base(features)
        {
            this.Client = client;
            this.Reqeust = request;
            this.Response = response;
        }

        public override string ToString()
        {
            return $"{this.Client} {this.Reqeust}";
        }
    }
}
