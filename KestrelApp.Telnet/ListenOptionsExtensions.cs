using KestrelApp.Telnet;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace KestrelApp
{
    /// <summary>
    ///  ListenOptions扩展
    /// </summary>
    public static class ListenOptionsExtensions
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
