using System;

namespace KestrelFramework.Pipelines
{
    /// <summary>
    /// 定义中间件管道创建者的接口
    /// </summary>
    /// <typeparam name="TContext">中间件上下文</typeparam>
    public interface IPipelineBuilder<TContext>
    {
        /// <summary>
        /// 获取应用服务提供者
        /// </summary>
        IServiceProvider ApplicationServices { get; }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <param name="middleware">中间件</param>
        /// <returns></returns>
        IPipelineBuilder<TContext> Use(Func<InvokeDelegate<TContext>, InvokeDelegate<TContext>> middleware);

        /// <summary>
        /// 创建所有中间件执行处理者
        /// </summary>
        /// <returns></returns>
        InvokeDelegate<TContext> Build();

        /// <summary>
        /// 使用默认配制创建新的PipelineBuilder
        /// </summary>
        /// <returns></returns>
        IPipelineBuilder<TContext> New();
    }
}
