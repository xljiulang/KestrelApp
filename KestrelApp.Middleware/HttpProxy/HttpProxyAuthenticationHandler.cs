using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace KestrelApp.Middleware.HttpProxy
{
    /// <summary>
    /// 代理身份认证
    /// </summary>
    sealed class HttpProxyAuthenticationHandler : IHttpProxyAuthenticationHandler
    { 
        public ValueTask<bool> AuthenticateAsync(AuthenticationHeaderValue? authentication)
        {
            return ValueTask.FromResult(true);
        }
    }
}
