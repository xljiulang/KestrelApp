using KestrelFramework.Application;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.Middlewares
{
    /// <summary>
    /// 回退处理中间件
    /// </summary>
    sealed class FallbackMiddlware : IRedisMiddleware
    {
        private readonly ILogger<FallbackMiddlware> logger;

        public FallbackMiddlware(ILogger<FallbackMiddlware> logger)
        {
            this.logger = logger;
        }

        public Task InvokeAsync(ApplicationDelegate<RedisContext> next, RedisContext context)
        {
            this.logger.LogWarning($"无法处理{context.Cmd}");
            return context.Client.ResponseAsync(RedisResponse.Err);
        }
    }
}
