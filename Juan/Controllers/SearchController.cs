using Juan.Data;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class SearchController : Controller
{
    private readonly JuanDbContext _context;

    public SearchController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? query)
    {
        if (query == null) return BadRequest();
        ViewBag.Query = query;
        IEnumerable<Product> products = await _context.Products.Where(p => p.Name.ToLower().Contains(query.ToLower())).AsNoTracking().ToListAsync();
        return View(products);
    }
}
