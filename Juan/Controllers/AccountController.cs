using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class AccountController : Controller
{
    private readonly JuanDbContext _context;

    public AccountController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated) return RedirectToAction("login", "authentication", new { returnUrl = "/account" });
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.OrderItems).AsNoTracking().ToListAsync();
        AccountVM accountVM = new()
        {
            Orders = orders,
        };
        return View(accountVM);
    }
}
