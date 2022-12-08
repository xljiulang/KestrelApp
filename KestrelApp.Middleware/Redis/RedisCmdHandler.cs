using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis命令处理者
    /// </summary>
    abstract class RedisCmdHandler : IRedisCmdHanler
    {
        /// <summary>
        /// 获取能处理的cmd名称
        /// </summary>
        public abstract RedisCmdName CmdName { get; }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task HandleAsync(RedisContext context)
        {
            return this.HandleAsync(context.Client, context.Reqeust);
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected abstract Task HandleAsync(RedisClient client, RedisCmd cmd);
    }
}
