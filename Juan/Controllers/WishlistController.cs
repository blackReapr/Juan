using Juan.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Juan.Controllers;

public class WishlistController : Controller
{
    public IActionResult Index()
    {
        IEnumerable<WishlistVM>? wishlist = JsonSerializer.Deserialize<IEnumerable<CartVM>>(Request.Cookies["wishlist"]);
        if (wishlist == null) wishlist = new List<WishlistVM>();
        return View();
    }
}
