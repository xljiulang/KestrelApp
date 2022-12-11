using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler.Middlewares
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
        /// 分析代理的http流量
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var feature = context.Features.Get<IProxyFeature>();
            if (feature == null || feature.ProxyProtocol == ProxyProtocol.None)
            {
                await next(context);
                return;
            }

            context.Request.EnableBuffering();
            var oldBody = context.Response.Body;
            using var response = new FileResponse();

            try
            {
                // 替换response的body
                context.Response.Body = response.Body;

                // 请求下个中间件
                await next(context);

                // 处理分析
                await this.AnalyzeAsync(context);
            }
            finally
            {
                response.Body.Position = 0L;
                await response.Body.CopyToAsync(oldBody);
                context.Response.Body = oldBody;
            }
        }

        private async ValueTask AnalyzeAsync(HttpContext context)
        {
            foreach (var item in this.analyzers)
            {
                context.Request.Body.Position = 0L;
                context.Response.Body.Position = 0L;
                await item.AnalyzeAsync(context);
            }
        }


        private class FileResponse : IDisposable
        {
            private readonly string filePath = Path.GetTempFileName();

            public Stream Body { get; }

            public FileResponse()
            {
                this.Body = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            }

            public void Dispose()
            {
                this.Body.Dispose();
                File.Delete(filePath);
            }
        }
    }
}
