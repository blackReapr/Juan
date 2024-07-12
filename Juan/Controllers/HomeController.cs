using Microsoft.AspNetCore.Mvc;

namespace Juan.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
