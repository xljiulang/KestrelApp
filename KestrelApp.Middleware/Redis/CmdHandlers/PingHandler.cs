using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Ping处理者
    /// </summary>
    sealed class PingHandler : IRedisCmdHanler
    {
        public RedisCmd Cmd => RedisCmd.Ping;

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param> 
        /// <returns></returns>
        public async ValueTask HandleAsync(RedisContext context)
        {
            await context.Response.WriteAsync(ResponseContent.Pong);
        }
    }
}
