using KestrelApp.Middleware.Redis.Middlewares;
using KestrelFramework.Application;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis连接处理者
    /// </summary>
    sealed class RedisConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<RedisConnectionHandler> logger;
        private readonly ApplicationDelegate<RedisContext> application;

        /// <summary>
        /// Redis连接处理者
        /// </summary> 
        /// <param name="appServices"></param> 
        /// <param name="logger"></param>
        public RedisConnectionHandler(
            IServiceProvider appServices,
            ILogger<RedisConnectionHandler> logger)
        {
            this.logger = logger;
            this.application = new AppliactionBuilder<RedisContext>(appServices)
                .Use<AuthMiddleware>()
                .Use<CmdMiddleware>()
                .Use<FallbackMiddlware>()
                .Build();
        }

        /// <summary>
        /// 处理Redis连接
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task OnConnectedAsync(ConnectionContext context)
        {
            try
            {
                var client = new RedisClient(context, this.application);
                await client.ProcessRedisAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogDebug(ex.Message);
            }
            finally
            {
                await context.DisposeAsync();
            }
        }
    }
}
