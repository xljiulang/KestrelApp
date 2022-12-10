using KestrelApp.Fiddler;
using KestrelApp.Fiddler.Certs;
using KestrelApp.Fiddler.Certs.CaCertInstallers;
using KestrelApp.Fiddler.HttpAnalyzers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ServiceCollection扩展
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Fiddler
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddFiddler(this IServiceCollection services)
        {
            var descriptor = ServiceDescriptor.Singleton<IHttpAnalyzer, LoggingHttpAnalyzer>();
            services.TryAddEnumerable(descriptor);

            services
                .AddMemoryCache()
                .AddSingleton<CertService>()
                .AddSingleton<ICaCertInstaller, CaCertInstallerOfMacOS>()
                .AddSingleton<ICaCertInstaller, CaCertInstallerOfWindows>()
                .AddSingleton<ICaCertInstaller, CaCertInstallerOfLinuxRedHat>()
                .AddSingleton<ICaCertInstaller, CaCertInstallerOfLinuxDebian>();

            return services.AddHttpForwarder();
        }
    }
}
