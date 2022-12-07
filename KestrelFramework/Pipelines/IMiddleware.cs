using System.Threading.Tasks;

namespace KestrelFramework.Pipelines
{
    /// <summary>
    /// 定义中间件的接口
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IMiddleware<TContext>
    {
        /// <summary>
        /// 执行中间件
        /// </summary>
        /// <param name="next">下一个中间件</param>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        Task InvokeAsync(InvokeDelegate<TContext> next, TContext context);
    }
}
