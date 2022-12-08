using KestrelFramework.Application;
using Microsoft.AspNetCore.Connections;
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
        private readonly ApplicationDelegate<RedisContext> application;

        /// <summary>
        /// 获取或设置是否已授权
        /// </summary>
        public bool? IsAuthed { get; set; }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        public EndPoint? RemoteEndPoint => context.RemoteEndPoint;

        /// <summary>
        /// Redis客户端
        /// </summary>
        /// <param name="context"></param> 
        /// <param name="application"></param>
        public RedisClient(ConnectionContext context, ApplicationDelegate<RedisContext> application)
        {
            this.context = context;
            this.application = application;
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
                    await this.application(context);
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
        public async Task ResponseAsync(IRedisResponse response)
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
