using System.Threading.Tasks;

namespace KestrelFramework.Application
{
    /// <summary>
    /// 表示可以处理应用请求的委托
    /// </summary>
    /// <typeparam name="TContext">中间件上下文类型</typeparam>
    /// <param name="context">中间件上下文</param>
    /// <returns></returns>
    public delegate Task ApplicationDelegate<TContext>(TContext context);
}
