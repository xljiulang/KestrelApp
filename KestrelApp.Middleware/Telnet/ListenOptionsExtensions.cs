using KestrelApp.Middleware.Telnet;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    ///  ListenOptions扩展
    /// </summary>
    public static partial class ListenOptionsExtensions
    {
        /// <summary>
        /// 使用TelnetConnectionHandler
        /// </summary>
        /// <param name="listen"></param>
        public static void UseTelnet(this ListenOptions listen)
        {
            listen.UseConnectionHandler<TelnetConnectionHandler>();
        }
    }
}
