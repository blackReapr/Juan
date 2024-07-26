using Microsoft.AspNetCore.Mvc;

namespace Juan.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated) return RedirectToAction("login", "authentication", new { returnUrl = "/account" });
        return View();
    }
}
