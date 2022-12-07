using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// select命令处理
    /// </summary>
    sealed class SelectHandler : CmdHandler
    {
        /// <summary>
        /// 是否能处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanHandle(RedisContext context)
        {
            return context.Cmd.Name == RedisCmdName.Select;
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override Task HandleAsync(RedisClient client, RedisCmd cmd)
        {
            return client.ResponseAsync(RedisResponse.OK);
        }
    }
}
