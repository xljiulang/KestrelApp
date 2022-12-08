using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// 退出
    /// </summary>
    sealed class QiuitHandler : RedisCmdHandler
    {
        public override RedisCmdName CmdName => RedisCmdName.Quit;
         
        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override Task HandleAsync(RedisClient client, RedisCmd cmd)
        {
            client.Close();
            return Task.CompletedTask;
        }
    }
}
