using KestrelApp.Fiddler.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseFiddler(this IApplicationBuilder app)
        { 
            app.UseMiddleware<HttpAnalyzeMiddleware>();
            app.UseMiddleware<HttpProxyMiddleware>();
        }
    }
}
