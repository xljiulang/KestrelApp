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
            this.application = new ApplicationBuilder<RedisContext>(appServices)
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
                await this.HandleRequestsAsync(context);
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

        /// <summary>
        /// 处理redis请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task HandleRequestsAsync(ConnectionContext context)
        {
            var input = context.Transport.Input;
            var client = new RedisClient(context);
            var response = new RedisResponse(context.Transport.Output);

            while (context.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                var requests = RedisRequest.Parse(result.Buffer, out var consumed);
                if (requests.Count > 0)
                {
                    foreach (var request in requests)
                    {
                        var redisContext = new RedisContext(client, request, response, context.Features);
                        await this.application.Invoke(redisContext);
                    }
                    input.AdvanceTo(consumed);
                }
                else
                {
                    input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }

    }
}
