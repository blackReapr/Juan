using Juan.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class CartController : Controller
{
    private readonly JuanDbContext _context;

    public CartController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> AddToCart(int? id)
    {
        if (id == null) return BadRequest();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return BadRequest();
        return View();
    }
}
