using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
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
            await WriteRequestAsync(context, writer);
            writer.WriteLine("[RESPONSE]");
            await WriteReponseAsync(context, writer);

            this.logger.LogInformation(builder.ToString());
        }


        private static async ValueTask WriteRequestAsync(HttpContext context, TextWriter writer)
        {
            var request = context.Request;
            await writer.WriteLineAsync($"{request.Method} {request.GetEncodedPathAndQuery()} {request.Protocol}");

            foreach (var header in request.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}:{header.Value}");
            }

            using var reader = new BodyReader(request.Body);
            var body = await reader.ReadToEndAsync();
            await writer.WriteLineAsync(body);
        }

        private static async ValueTask WriteReponseAsync(HttpContext context, TextWriter writer)
        {
            var response = context.Response;
            var feature = context.Features.Get<IHttpResponseFeature>();

            await writer.WriteLineAsync($"{context.Request.Protocol} {response.StatusCode} {feature?.ReasonPhrase}");
            foreach (var header in response.Headers)
            {
                await writer.WriteLineAsync($"{header.Key}:{header.Value}");
            }

            using var reader = new BodyReader(response.Body);
            var body = await reader.ReadToEndAsync();
            await writer.WriteLineAsync(body);
        }

        private class BodyReader : StreamReader
        {
            public BodyReader(Stream stream, bool gzip = false)
                : base(stream)
            {
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}
