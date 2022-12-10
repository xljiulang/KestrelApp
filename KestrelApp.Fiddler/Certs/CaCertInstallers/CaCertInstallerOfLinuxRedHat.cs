using Microsoft.Extensions.Logging;

namespace KestrelApp.Fiddler.Certs.CaCertInstallers
{
    sealed class CaCertInstallerOfLinuxRedHat : CaCertInstallerOfLinux
    {
        protected override string CaCertUpdatePath => "/usr/bin/update-ca-trust";

        protected override string CaCertStorePath => "/etc/pki/ca-trust/source/anchors";

        public CaCertInstallerOfLinuxRedHat(ILogger<CaCertInstallerOfLinuxRedHat> logger)
            : base(logger)
        {
        }
    }
}