using Microsoft.AspNetCore.Mvc;

namespace Juan.Controllers;

public class ShopController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
