using Microsoft.AspNetCore.Mvc;

namespace KestrelApp.Fiddler.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return this.Content("欢迎使用(伪)Fiddler，请设置系统或浏览器代理地址到：http://127.0.0.1:5000");
        }
    }
}
