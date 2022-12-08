using KestrelFramework.Application;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.Middlewares
{
    /// <summary>
    /// 命令处理中间件
    /// </summary>
    sealed class CmdMiddleware : IRedisMiddleware
    {
        private readonly Dictionary<RedisCmdName, IRedisCmdHanler> cmdHandlers;

        public CmdMiddleware(IEnumerable<IRedisCmdHanler> cmdHanlers)
        {
            this.cmdHandlers = cmdHanlers.ToDictionary(item => item.CmdName, item => item);
        }

        public async Task InvokeAsync(ApplicationDelegate<RedisContext> next, RedisContext context)
        {
            if (this.cmdHandlers.TryGetValue(context.Reqeust.Name, out var hanler))
            {
                await hanler.HandleAsync(context);
            }
            else
            {
                await next(context);
            }
        }
    }
}
