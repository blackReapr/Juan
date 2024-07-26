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

    public async Task<IActionResult> Index(string? category, string? color, string? size, int page = 1)
    {
        IEnumerable<Size> sizes = await _context.Sizes.AsNoTracking().ToListAsync();
        IEnumerable<Color> colors = await _context.Colors.AsNoTracking().ToListAsync();
        IQueryable<Product> query = _context.Products;

        if (color != null) query = query.Where(p => p.ProductColors.Any(pc => pc.Color.Name == color));
        if (size != null) query = query.Where(p => p.ProductSizes.Any(ps => ps.Size.Name == color));
        if (category != null) query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Name == color));

        IEnumerable<Product> products = await query.ToListAsync();
        IEnumerable<Category> categories = await _context.Categories.AsNoTracking().ToListAsync();

        List<IDictionary<string, string>> productCategories = new();
        foreach (Category categoryFor in categories)
        {
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs["name"] = categoryFor.Name;
            keyValuePairs["count"] = _context.ProductCategories.Count(pc => pc.CategoryId == categoryFor.Id).ToString();
            productCategories.Add(keyValuePairs);
        }

        List<IDictionary<string, string>> productColors = new();
        foreach (Color colorFor in colors)
        {
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs["name"] = colorFor.Name;
            keyValuePairs["count"] = _context.ProductColors.Count(pc => pc.ColorId == colorFor.Id).ToString();
            productColors.Add(keyValuePairs);
        }

        List<IDictionary<string, string>> productSizes = new();
        foreach (Size sizeFor in sizes)
        {
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs["name"] = sizeFor.Name;
            keyValuePairs["count"] = _context.ProductSizes.Count(ps => ps.SizeId == sizeFor.Id).ToString();
            productSizes.Add(keyValuePairs);
        }

        ShopVM shopVM = new()
        {
            Categories = categories,
            Products = await PaginationVM<Product>.CreateAsync(query, page, 9),
            Colors = colors,
            Sizes = sizes,
            ProductColors = productColors,
            ProductCategories = productCategories,
            ProductSizes = productSizes,
        };
        return View(shopVM);
    }
}
