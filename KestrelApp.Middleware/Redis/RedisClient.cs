using KestrelFramework.Pipelines;
using Microsoft.AspNetCore.Connections;
using System;
using System.Net;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.Redis
{
    /// <summary>
    /// 表示Redis客户端
    /// </summary>
    sealed class RedisClient
    {
        private readonly ConnectionContext context;
        private readonly InvokeDelegate<RedisContext> invokeDelegate;

        /// <summary>
        /// 获取或设置是否已授权
        /// </summary>
        public bool? IsAuthed { get; set; }

        /// <summary>
        /// 获取连接时间
        /// </summary>
        public DateTime ConnectedTime { get; } = DateTime.Now;

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public EndPoint? RemoteEndPoint => context.RemoteEndPoint;

        /// <summary>
        /// Redis客户端
        /// </summary>
        /// <param name="context"></param> 
        /// <param name="invokeDelegate"></param>
        public RedisClient(ConnectionContext context, InvokeDelegate<RedisContext> invokeDelegate)
        {
            this.context = context;
            this.invokeDelegate = invokeDelegate;
        }

        /// <summary>
        /// 处理redis请求
        /// </summary>
        /// <returns></returns>
        public async Task ProcessRedisAsync()
        {
            var input = this.context.Transport.Input;
            while (this.context.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                var cmds = RedisCmd.Parse(result.Buffer, out var consumed);
                if (cmds.Count > 0)
                {
                    input.AdvanceTo(consumed);
                }
                else
                {
                    input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }

                foreach (var cmd in cmds)
                {
                    var context = new RedisContext(this, cmd);
                    await this.invokeDelegate(context);
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task ResponseAsync(RedisResponse response)
        {
            await this.context.Transport.Output.WriteAsync(response.ToMemory());
        }


        public void Close()
        {
            this.context.Abort();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return this.RemoteEndPoint?.ToString();
        }
    }
}
