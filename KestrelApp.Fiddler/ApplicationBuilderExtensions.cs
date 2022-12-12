using KestrelApp.Fiddler.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 使用Fiddler的http中间件
        /// </summary>
        /// <param name="app"></param>
        public static void UseFiddler(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpAnalyzeMiddleware>();
            app.UseMiddleware<HttpForwardMiddleware>();
        }
    }
}
