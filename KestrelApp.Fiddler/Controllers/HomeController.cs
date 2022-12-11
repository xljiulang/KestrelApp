using Microsoft.AspNetCore.Mvc;

namespace KestrelApp.Fiddler.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return this.Content("欢迎使用Fiddler，请在远程客户端设备上安装Fiddler的跟证书");
        }
    }
}
