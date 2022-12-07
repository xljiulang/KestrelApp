using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 定义redis命令处理者
    /// </summary>
    interface ICmdHanler
    {
        /// <summary>
        /// 返回是否可以处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool CanHandle(RedisContext context);

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task HandleAsync(RedisContext context);
    }
}
