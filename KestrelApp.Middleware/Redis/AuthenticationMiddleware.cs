using KestrelFramework.Application;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 认证中间件
    /// </summary>
    sealed class AuthenticationMiddleware : IApplicationMiddleware<RedisContext>
    {
        private readonly IOptionsMonitor<RedisOptions> options;

        public AuthenticationMiddleware(IOptionsMonitor<RedisOptions> options)
        {
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
            else if (context.Cmd.Name != RedisCmdName.Auth)
            {
                if (string.IsNullOrEmpty(this.options.CurrentValue.Auth))
                {
                    context.Client.IsAuthed = true;
                    await next(context);
                }
                else
                {
                    // 这里应该要提示需要Auth信息
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
