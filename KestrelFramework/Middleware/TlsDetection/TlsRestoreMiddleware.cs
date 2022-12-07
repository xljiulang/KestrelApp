using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.Tasks;

namespace KestrelFramework.Middleware.TlsDetection
{
    /// <summary>
    /// tls恢复中间件
    /// </summary>
    sealed class TlsRestoreMiddleware : IKestrelMiddleware
    {
        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(ConnectionDelegate next, ConnectionContext context)
        {
            if (context.Features.Get<ITlsConnectionFeature>() == FakeTlsConnectionFeature.Instance)
            {
                // 擦除入侵
                context.Features.Set<ITlsConnectionFeature>(null);
            }
            await next(context);
        }
    }
}
