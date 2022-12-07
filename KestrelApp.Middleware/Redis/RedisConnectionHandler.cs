using KestrelFramework.Pipelines;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis连接处理者
    /// </summary>
    sealed class RedisConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<RedisConnectionHandler> logger;
        private readonly InvokeDelegate<RedisContext> invokeDelegate;

        /// <summary>
        /// Redis连接处理者
        /// </summary> 
        /// <param name="appServices"></param>
        /// <param name="cmdHanlers"></param> 
        /// <param name="logger"></param>
        public RedisConnectionHandler(
            IServiceProvider appServices,
            IEnumerable<ICmdHanler> cmdHanlers,
            ILogger<RedisConnectionHandler> logger)
        {
            this.logger = logger;

            var builder = new PipelineBuilder<RedisContext>(appServices, context =>
            {
                return context.Client.ResponseAsync(RedisResponse.Err);
            });

            builder.Use<AuthMiddleware>();

            // 添加cmd条件分支
            foreach (var cmd in cmdHanlers)
            {
                builder.When(cmd.CanHandle, cmd.HandleAsync);
            }

            this.invokeDelegate = builder.Build();
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
                var client = new RedisClient(context, this.invokeDelegate);
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
