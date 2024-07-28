using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class AccountController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(JuanDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated) return RedirectToAction("login", "authentication", new { returnUrl = "/account" });
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return BadRequest();
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == appUser.Id).AsNoTracking().ToListAsync();
        AccountVM accountVM = new()
        {
            Orders = orders,
        };
        return View(accountVM);
    }
}
