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
        public async Task ProcessRequestAsync()
        {
            var input = this.context.Transport.Input;
            while (this.context.ConnectionClosed.IsCancellationRequested == false)
            {
                var result = await input.ReadAsync();
                if (result.IsCanceled)
                {
                    break;
                }

                var requests = RedisRequest.Parse(result.Buffer, out var consumed);
                if (requests.Count > 0)
                {
                    input.AdvanceTo(consumed);
                }
                else
                {
                    input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                }

                foreach (var request in requests)
                {
                    var redisContext = new RedisContext(this, request, this.context);
                    await this.application.Invoke(redisContext);
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
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
