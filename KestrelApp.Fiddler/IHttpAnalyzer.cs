using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;

namespace KestrelApp.Fiddler
{
    /// <summary>
    /// http分析器
    /// 支持多个实例
    /// </summary>
    public interface IHttpAnalyzer
    {
        /// <summary>
        /// 分析http
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        ValueTask AnalyzeAsync(HttpRequest request, HttpResponse response, FileBufferingWriteStream responseBody);
    }
}
