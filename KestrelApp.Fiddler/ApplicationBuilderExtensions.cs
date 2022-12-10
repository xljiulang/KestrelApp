using KestrelApp.Fiddler.Http;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseFiddler(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpHomeMiddleware>();
            app.UseMiddleware<HttpAnalyzeMiddleware>();
            app.UseMiddleware<HttpProxyMiddleware>();
        }
    }
}
