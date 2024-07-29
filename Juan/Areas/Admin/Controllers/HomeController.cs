using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Juan.Areas.Admin.Controllers;

[Area("admin"), Authorize(Roles = "admin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
