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

    public async Task<IActionResult> Index(string? category, string? color, string? size, string? amount, string? sortBy, int page = 1)
    {
        IEnumerable<Size> sizes = await _context.Sizes.AsNoTracking().ToListAsync();
        IEnumerable<Color> colors = await _context.Colors.AsNoTracking().ToListAsync();
        IQueryable<Product> query = _context.Products;

        if (color != null) query = query.Where(p => p.ProductColors.Any(pc => pc.Color.Name == color));
        if (size != null) query = query.Where(p => p.ProductSizes.Any(ps => ps.Size.Name == color));
        if (category != null) query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Name == color));
        if (amount != null)
        {
            string v = amount.Split("-")[0].Trim();
            v = v.Remove(0, 1);
            int min = Convert.ToInt32(v);
            string x = amount.Split("-")[1].Trim();
            x = x.Remove(0, 1);
            int max = Convert.ToInt32(x);
            query = query.Where(p => (p.DiscountPrice > 0 ? (p.DiscountPrice >= min && p.DiscountPrice <= max) : (p.Price >= min && p.Price <= max)));
        }
        if (sortBy != null)
        {
            if (sortBy == "date") query = query.OrderByDescending(p => p.CreatedAt);
            else if (sortBy == "name-asc") query = query.OrderBy(p => p.Name);
            else if (sortBy == "name-desc") query = query.OrderByDescending(p => p.Name);
            else if (sortBy == "price-asc") query = query.OrderBy(p => p.DiscountPrice > 0 ? p.DiscountPrice : p.Price);
            else if (sortBy == "price-desc") query = query.OrderByDescending(p => p.DiscountPrice > 0 ? p.DiscountPrice : p.Price);
            else if (sortBy == "rating-asc") query = query.OrderBy(p => p.Reviews.Average(r => r.Rating));
            else if (sortBy == "rating-desc") query = query.OrderByDescending(p => p.Reviews.Average(r => r.Rating));
        }

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
            Products = await PaginationVM<Product>.CreateAsync(query, page, 9, category, color, size, amount, sortBy),
            Colors = colors,
            Sizes = sizes,
            ProductColors = productColors,
            ProductCategories = productCategories,
            ProductSizes = productSizes,
        };
        return View(shopVM);
    }
}
