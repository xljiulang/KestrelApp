using KestrelFramework.Application;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis.Middlewares
{
    /// <summary>
    /// 认证中间件
    /// </summary>
    sealed class AuthMiddleware : IRedisMiddleware
    {
        private readonly ILogger<AuthMiddleware> logger;
        private readonly IOptionsMonitor<RedisOptions> options;

        public AuthMiddleware(
            ILogger<AuthMiddleware> logger,
            IOptionsMonitor<RedisOptions> options)
        {
            this.logger = logger;
            this.options = options;
        }

        public async Task InvokeAsync(ApplicationDelegate<RedisContext> next, RedisContext context)
        {
            if (context.Client.IsAuthed == false)
            {
                await context.Client.ResponseAsync(RedisResponse.Err);
            }
            else if (context.Client.IsAuthed == true)
            {
                await next(context);
            }
            else if (context.Reqeust.Name != RedisCmdName.Auth)
            {
                if (string.IsNullOrEmpty(options.CurrentValue.Auth))
                {
                    context.Client.IsAuthed = true;
                    await next(context);
                }
                else
                {
                    this.logger.LogWarning("需要客户端Auth");
                    await context.Client.ResponseAsync(RedisResponse.Err);
                }
            }
            else
            {
                await next(context);
            }
        }
    }
}
