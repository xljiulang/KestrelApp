using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Ping处理者
    /// </summary>
    sealed class PingHandler : CmdHandler
    {
        /// <summary>
        /// 是否能处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanHandle(RedisContext context)
        {
            return context.Cmd.Name == RedisCmdName.Ping;
        }

        /// <summary>
        /// 处理ping
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override Task HandleAsync(RedisClient client, RedisCmd cmd)
        {
            return client.ResponseAsync(RedisResponse.Pong);
        }
    }
}
