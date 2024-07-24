using Microsoft.AspNetCore.Mvc;

namespace Juan.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
