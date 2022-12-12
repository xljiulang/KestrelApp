using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
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

        public async ValueTask AnalyzeAsync(HttpContext context)
        {
            var builder = new StringBuilder();
            var writer = new StringWriter(builder);

            writer.WriteLine("[REQUEST]");
            await context.SerializeRequestAsync(writer);

            writer.WriteLine("[RESPONSE]");
            await context.SerializeResponseAsync(writer);

            this.logger.LogInformation(builder.ToString());
        }
    }
}
