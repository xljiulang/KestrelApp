using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// Ping处理者
    /// </summary>
    sealed class PingHandler : RedisCmdHandler
    {
        public override RedisCmdName CmdName => RedisCmdName.Ping;
         

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
