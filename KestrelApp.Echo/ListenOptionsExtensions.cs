using KestrelApp.Echo;
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
        /// 使用EchoConnectionHandler
        /// </summary>
        /// <param name="listen"></param>
        public static void UseEcho(this ListenOptions listen)
        {
            listen.UseConnectionHandler<EchoConnectionHandler>();
        }
    }
}
