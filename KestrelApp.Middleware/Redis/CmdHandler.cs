using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis命令处理者
    /// </summary>
    abstract class CmdHandler : ICmdHanler
    {
        /// <summary>
        /// 返回是否可以处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool CanHandle(RedisContext context);

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task HandleAsync(RedisContext context)
        {
            return this.HandleAsync(context.Client, context.Cmd);
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
