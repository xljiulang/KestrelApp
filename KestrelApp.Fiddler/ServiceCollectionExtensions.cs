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
        /// 添加Fiddler依赖项
        /// </summary>
        /// <param name="services"></param> 
        /// <returns></returns>
        public static IServiceCollection AddFiddler(this IServiceCollection services)
        {
            services.TryAddSingleton<CertService>();
            services.AddSingleton<ICaCertInstaller, CaCertInstallerOfMacOS>();
            services.AddSingleton<ICaCertInstaller, CaCertInstallerOfWindows>();
            services.AddSingleton<ICaCertInstaller, CaCertInstallerOfLinuxRedHat>();
            services.AddSingleton<ICaCertInstaller, CaCertInstallerOfLinuxDebian>();

            services.TryAddSingleton<IHttpAnalyzer, LoggingHttpAnalyzer>();
            return services.AddMemoryCache().AddHttpForwarder();
        }
    }
}
