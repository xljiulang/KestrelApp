using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.HttpProxy
{
    /// <summary>
    /// http代理身份认证处理者
    /// </summary>
    public interface IHttpProxyAuthenticationHandler
    {
        /// <summary>
        /// 认证身份
        /// </summary>
        /// <param name="authentication"></param>
        /// <returns></returns>
        ValueTask<bool> AuthenticateAsync(AuthenticationHeaderValue? authentication);
    }
}
