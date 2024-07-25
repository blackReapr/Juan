using Juan.Data;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class ProductController : Controller
{
    private readonly JuanDbContext _context;

    public ProductController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Modal(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return NotFound();
        return PartialView("_ProductModalPartial", product);
    }

    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products
            .Include(p => p.ProductColors)
            .ThenInclude(c => c.Color)
            .Include(p => p.ProductSizes)
            .ThenInclude(s => s.Size)
            .Include(p => p.ProductImages)
            .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }
}
