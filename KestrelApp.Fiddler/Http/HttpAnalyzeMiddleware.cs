using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler.Http
{
    /// <summary>
    /// http分析中间件
    /// </summary>
    sealed class HttpAnalyzeMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IEnumerable<IHttpAnalyzer> analyzers;

        /// <summary>
        /// http分析中间件
        /// </summary>
        /// <param name="next"></param>
        /// <param name="analyzers"></param> 
        public HttpAnalyzeMiddleware(
            RequestDelegate next,
            IEnumerable<IHttpAnalyzer> analyzers)
        {
            this.next = next;
            this.analyzers = analyzers;
        }

        /// <summary>
        /// 处理http代理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            var oldBody = context.Response.Body;
            using var newBody = new FileBufferingWriteStream();
            try
            {
                // 替换respone的body
                context.Response.Body = newBody;

                // 请求下个中间件
                await next(context);

                // 处理分析
                await this.AnalyzeAsync(context.Request, context.Response, newBody);

                // 处理响应
                await newBody.DrainBufferAsync(oldBody);
            }
            finally
            {
                // 恢复body
                context.Response.Body = oldBody;
            }
        }

        private async ValueTask AnalyzeAsync(HttpRequest request, HttpResponse response, FileBufferingWriteStream responseBody)
        {
            foreach (var item in this.analyzers)
            {
                request.Body.Position = 0L;
                await item.AnalyzeAsync(request, response, responseBody);
            }
        }
    }
}
