using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace KestrelFramework.Pipelines
{
    /// <summary>
    /// 中间件创建者扩展
    /// </summary>
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// 中断执行中间件
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="handler">处理者</param>
        /// <returns></returns>
        public static IPipelineBuilder<TContext> Run<TContext>(this IPipelineBuilder<TContext> builder, InvokeDelegate<TContext> handler)
        {
            return builder.Use(next => handler);
        }

        /// <summary>
        /// 条件中间件
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="predicate"></param>
        /// <param name="handler"></param> 
        /// <returns></returns>
        public static IPipelineBuilder<TContext> When<TContext>(this IPipelineBuilder<TContext> builder, Func<TContext, bool> predicate, InvokeDelegate<TContext> handler)
        {
            return builder.Use(next => async context =>
            {
                if (predicate(context))
                {
                    await handler(context);
                }
                else
                {
                    await next(context);
                }
            });
        }


        /// <summary>
        /// 条件中间件
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="predicate"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public static IPipelineBuilder<TContext> When<TContext>(this IPipelineBuilder<TContext> builder, Func<TContext, bool> predicate, Action<IPipelineBuilder<TContext>> configureAction)
        {
            return builder.Use(next => async context =>
            {
                if (predicate(context))
                {
                    var branchBuilder = builder.New();
                    configureAction(branchBuilder);
                    await branchBuilder.Build().Invoke(context);
                }
                else
                {
                    await next(context);
                }
            });
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IPipelineBuilder<TContext> Use<TContext, TMiddleware>(this IPipelineBuilder<TContext> builder)
            where TMiddleware : IMiddleware<TContext>
        {
            var middleware = ActivatorUtilities.GetServiceOrCreateInstance<TMiddleware>(builder.ApplicationServices);
            return builder.Use(middleware);
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TMiddleware"></typeparam>
        /// <param name="builder"></param>
        /// <param name="middleware"></param>
        /// <returns></returns>
        public static IPipelineBuilder<TContext> Use<TContext, TMiddleware>(this IPipelineBuilder<TContext> builder, TMiddleware middleware)
            where TMiddleware : IMiddleware<TContext>
        {
            return builder.Use(middleware.InvokeAsync);
        }

        /// <summary>
        /// 使用中间件
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="builder"></param>
        /// <param name="middleware"></param>
        /// <returns></returns>
        public static IPipelineBuilder<TContext> Use<TContext>(this IPipelineBuilder<TContext> builder, Func<InvokeDelegate<TContext>, TContext, Task> middleware)
        {
            return builder.Use(next => context => middleware(next, context));
        }
    }
}
