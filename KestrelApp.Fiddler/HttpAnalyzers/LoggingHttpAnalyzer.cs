using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler.HttpAnalyzers
{
    public class LoggingHttpAnalyzer : IHttpAnalyzer
    {
        private readonly ILogger<LoggingHttpAnalyzer> logger;

        public LoggingHttpAnalyzer(ILogger<LoggingHttpAnalyzer> logger)
        {
            this.logger = logger;
        }

        public ValueTask AnalyzeAsync(HttpRequest request, HttpResponse response, FileBufferingWriteStream responseBody)
        {
            //responseBody.DrainBufferAsync(..);

            this.logger.LogInformation($"请求到{request.Scheme}://{request.Host}{request.GetEncodedPathAndQuery()} 响应了{response.StatusCode}");
            return ValueTask.CompletedTask;
        }
    }
}
