using Microsoft.AspNetCore.Http;
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
        /// <param name="context"></param> 
        /// <returns></returns>
        ValueTask AnalyzeAsync(HttpContext context);
    }
}
