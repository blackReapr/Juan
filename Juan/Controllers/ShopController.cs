using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class ShopController : Controller
{
    private readonly JuanDbContext _context;

    public ShopController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Size> sizes = await _context.Sizes.AsNoTracking().ToListAsync();
        IEnumerable<Color> colors = await _context.Colors.AsNoTracking().ToListAsync();
        IEnumerable<Product> products = await _context.Products.AsNoTracking().ToListAsync();
        IEnumerable<Category> categories = await _context.Categories.AsNoTracking().ToListAsync();
        IEnumerable<ProductColor> productColors = await _context.ProductColors.AsNoTracking().ToListAsync();
        IEnumerable<ProductSize> productSizes = await _context.ProductSizes.AsNoTracking().ToListAsync();
        IEnumerable<ProductCategory> productCategories = await _context.ProductCategories.AsNoTracking().ToListAsync();
        ShopVM shopVM = new()
        {
            Categories = categories,
            Products = products,
            Colors = colors,
            Sizes = sizes,
            ProductColors = productColors,
            ProductCategories = productCategories,
            ProductSizes = productSizes,
        };
        return View(shopVM);
    }
}
