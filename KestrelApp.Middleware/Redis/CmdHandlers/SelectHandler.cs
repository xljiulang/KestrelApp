using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.CmdHandlers
{
    /// <summary>
    /// select命令处理
    /// </summary>
    sealed class SelectHandler : IRedisCmdHanler
    {
        public RedisCmd Cmd => RedisCmd.Select;

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="context"></param> 
        /// <returns></returns>
        public async ValueTask HandleAsync(RedisContext context)
        {
            await context.Response.WriteAsync(ResponseContent.OK);
        }
    }
}
