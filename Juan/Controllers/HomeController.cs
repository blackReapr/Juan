using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers
{
    public class HomeController : Controller
    {
        private readonly JuanDbContext _context;

        public HomeController(JuanDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.ToListAsync();
            IEnumerable<Product> products = await _context.Products.ToListAsync();
            IEnumerable<Product> newProducts = await _context.Products.Where(p => p.IsNew).Take(4).ToListAsync();
            HomeVM homeVM = new()
            {
                Sliders = sliders,
                Products = products,
                NewProducts = newProducts
            };
            return View(homeVM);
        }
    }
}
