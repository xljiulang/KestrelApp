using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 定义redis命令处理者
    /// </summary>
    interface IRedisCmdHanler
    {
        /// <summary>
        /// 获取能处理的cmd名称
        /// </summary>
        RedisCmdName CmdName { get; }

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task HandleAsync(RedisContext context);
    }
}
