using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

[Authorize(Roles = "memeber"), AllowAnonymous]
public class HomeController : Controller
{
    private readonly JuanDbContext _context;

    public HomeController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        //for (int i = 1; i <= 12; i++)
        //{
        //    Product product = new()
        //    {
        //        Count = 10,
        //        Description = $"Description {i}",
        //        Name = $"Product {i}",
        //        MainImage = $"product-{i}.jpg",
        //        IsNew = i % 2 == 0,
        //        Price = i * 15,
        //        DiscountPrice = i * 12,
        //        DetailedDescription = $"Description {i}"
        //    };
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();
        //}
        IEnumerable<Slider> sliders = await _context.Sliders.AsNoTracking().ToListAsync();
        IEnumerable<Product> products = await _context.Products.AsNoTracking().ToListAsync();
        IEnumerable<Product> newProducts = await _context.Products.AsNoTracking().Where(p => p.IsNew).ToListAsync();
        IEnumerable<Blog> blogs = await _context.Blogs.AsNoTracking().Include(b => b.User).ToListAsync();
        HomeVM homeVM = new()
        {
            Sliders = sliders,
            Products = products,
            NewProducts = newProducts,
            Blogs = blogs
        };
        return View(homeVM);
    }
}
