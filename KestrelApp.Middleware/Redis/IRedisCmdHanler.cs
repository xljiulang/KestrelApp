using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 定义redis请求处理者
    /// </summary>
    interface IRedisCmdHanler
    {
        /// <summary>
        /// 获取能处理的请求命令
        /// </summary>
        RedisCmd Cmd { get; }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ValueTask HandleAsync(RedisContext context);
    }
}
