using Juan.Data;
using Juan.Extensions;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.admin.Controllers;

[Area("admin")]
public class ProductController : Controller
{
    private readonly JuanDbContext _context;

    public ProductController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Product> products = await _context.Products.ToListAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) return View(product);

        var mainFile = product.MainPhoto;
        var hoverFile = product.HoverPhoto;
        var files = product.Photos;

        if (mainFile == null)
        {
            ModelState.AddModelError(nameof(Product.MainPhoto), "Main image is required");
            return View(product);
        }
        if (hoverFile == null)
        {
            ModelState.AddModelError(nameof(Product.HoverPhoto), "Hover image is required");
            return View(product);
        }
        if (files == null)
        {
            ModelState.AddModelError(nameof(Product.Photos), "Images is required");
            return View(product);
        }


        if (!mainFile.IsImage())
        {
            ModelState.AddModelError(nameof(Product.MainPhoto), "Invalid format");
            return View(product);
        }
        if (mainFile.DoesExceed(100))
        {
            ModelState.AddModelError(nameof(Product.MainPhoto), "File limit exceeded");
            return View(product);
        }

        product.MainImage = await mainFile.SaveAsync();

        if (!hoverFile.IsImage())
        {
            ModelState.AddModelError(nameof(Product.HoverPhoto), "Invalid format");
            return View(product);
        }
        if (hoverFile.DoesExceed(100))
        {
            ModelState.AddModelError(nameof(Product.HoverPhoto), "File limit exceeded");
            return View(product);
        }
        product.HoverImage = await hoverFile.SaveAsync();


        List<ProductImage> images = new();
        foreach (IFormFile file in files)
        {
            if (!file.IsImage())
            {
                ModelState.AddModelError(nameof(Product.Photos), "Invalid format");
                return View(product);
            }
            if (file.DoesExceed(100))
            {
                ModelState.AddModelError(nameof(Product.Photos), "File limit exceeded");
                return View(product);
            }
            string filename = await file.SaveAsync();
            ProductImage image = new() { CreatedAt = DateTime.Now, Image = filename, ProductId = product.Id };
            images.Add(image);
        }
        product.ProductImages = images;

        product.CreatedAt = DateTime.Now;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
