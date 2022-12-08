using KestrelFramework.Application;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// redis中间件
    /// </summary>
    interface IRedisMiddleware : IApplicationMiddleware<RedisContext>
    {
    }
}
