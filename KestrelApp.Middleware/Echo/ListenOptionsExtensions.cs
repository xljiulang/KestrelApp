using KestrelApp.Middleware.Echo;
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
        /// 使用EchoConnectionHandler
        /// </summary>
        /// <param name="listen"></param>
        public static void UseEcho(this ListenOptions listen)
        {
            listen.UseConnectionHandler<EchoConnectionHandler>();
        }
    }
}
