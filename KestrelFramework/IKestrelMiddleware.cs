using Microsoft.AspNetCore.Connections;
using System.Threading.Tasks;

namespace KestrelFramework
{
    /// <summary>
    /// Kestrel的中间件接口
    /// </summary>
    public interface IKestrelMiddleware
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task InvokeAsync(ConnectionDelegate next, ConnectionContext context);
    }
}
