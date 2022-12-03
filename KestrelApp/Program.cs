using KestrelApp.HttpProxy;
using Serilog;

namespace KestrelApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddHttproxy();

            builder.Host.UseSerilog((hosting, logger) =>
            {
                logger.ReadFrom
                    .Configuration(hosting.Configuration)
                    .Enrich.FromLogContext().WriteTo.Console(outputTemplate: "{Timestamp:O} [{Level:u3}]{NewLine}{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}");
            });

            builder.WebHost.ConfigureKestrel((context, kestrel) =>
            {
                var section = context.Configuration.GetSection("Kestrel");
                kestrel.Configure(section)
                    .Endpoint("HttpProxy", endpoint => endpoint.ListenOptions.UseHttpProxy());
            });

            var app = builder.Build();
            app.UseRouting();
            app.UseMiddleware<HttpProxyMiddleware>();

            app.Run();
        }
    }
}