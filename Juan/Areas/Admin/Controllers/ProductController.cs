using Juan.Data;
using Juan.Extensions;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.Admin.Controllers;

[Area("admin"), Authorize(Roles = "admin")]
public class ProductController : Controller
{
    private readonly JuanDbContext _context;

    public ProductController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Product> products = await _context.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .Include(p => p.ProductColors)
            .ThenInclude(pc => pc.Color)
            .Include(p => p.ProductSizes)
            .ThenInclude(pc => pc.Size)
            .AsNoTracking()
            .ToListAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Colors = await _context.Colors.ToListAsync();
        ViewBag.Sizes = await _context.Sizes.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        ViewBag.Colors = await _context.Colors.ToListAsync();
        ViewBag.Sizes = await _context.Sizes.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        if (!ModelState.IsValid) return View(product);

        #region Many-to-Many
        List<ProductColor> productColors = new();

        foreach (int colorId in product.ColorIds)
        {
            Color? color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == colorId);
            if (color == null)
            {
                ModelState.AddModelError("ColorIds", "Invalid color id");
                return View(product);
            }
            ProductColor productColor = new()
            {
                ColorId = colorId,
                ProductId = product.Id,
            };
            productColors.Add(productColor);
        }

        List<ProductSize> productSizes = new();

        foreach (int sizeId in product.SizeIds)
        {
            Size? size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == sizeId);
            if (size == null)
            {
                ModelState.AddModelError("SizeIds", "Invalid size id");
                return View(product);
            }
            ProductSize productSize = new()
            {
                SizeId = sizeId,
                ProductId = product.Id,
            };
            productSizes.Add(productSize);
        }

        List<ProductCategory> productCategories = new();

        foreach (int categoryId in product.CategoryIds)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryIds", "Invalid category id");
                return View(product);
            }
            ProductCategory productCategory = new()
            {
                CategoryId = categoryId,
                ProductId = product.Id,
            };
            productCategories.Add(productCategory);
        }
        #endregion

        IEnumerable<string> errors = product.MainPhoto.VerifyFile();
        if (errors.Any())
        {
            foreach (string error in errors) ModelState.AddModelError("Photo", error);
            return View(product);
        }

        foreach (IFormFile file in product.Photos)
        {
            IEnumerable<string> fileErrors = product.MainPhoto.VerifyFile();
            if (fileErrors.Any())
            {
                foreach (string error in fileErrors) ModelState.AddModelError("Photo", error);
            }
            return View(product);
        }

        string filename = await product.MainPhoto.SaveFileAsync();
        List<ProductImage> images = new();
        foreach (IFormFile file in product.Photos)
        {
            string name = await file.SaveFileAsync();
            ProductImage image = new() { Image = name, ProductId = product.Id };
            images.Add(image);
        }

        product.ProductImages = images;
        product.MainImage = filename;
        product.ProductColors = productColors;
        product.ProductSizes = productSizes;
        product.ProductCategories = productCategories;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(int? id, Product product)
    {
        if (id == null || id != product.Id) return BadRequest();
        Product? existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        ViewBag.Colors = await _context.Colors.ToListAsync();
        ViewBag.Sizes = await _context.Sizes.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();
        if (!ModelState.IsValid) return View(product);

        #region Many-to-Many
        List<ProductColor> productColors = new();

        foreach (int colorId in product.ColorIds)
        {
            Color? color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == colorId);
            if (color == null)
            {
                ModelState.AddModelError("ColorIds", "Invalid color id");
                return View(product);
            }
            ProductColor productColor = new()
            {
                ColorId = colorId,
                ProductId = product.Id,
            };
            productColors.Add(productColor);
        }

        List<ProductSize> productSizes = new();

        foreach (int sizeId in product.SizeIds)
        {
            Size? size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == sizeId);
            if (size == null)
            {
                ModelState.AddModelError("SizeIds", "Invalid size id");
                return View(product);
            }
            ProductSize productSize = new()
            {
                SizeId = sizeId,
                ProductId = product.Id,
            };
            productSizes.Add(productSize);
        }

        List<ProductCategory> productCategories = new();

        foreach (int categoryId in product.CategoryIds)
        {
            Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryIds", "Invalid category id");
                return View(product);
            }
            ProductCategory productCategory = new()
            {
                CategoryId = categoryId,
                ProductId = product.Id,
            };
            productCategories.Add(productCategory);
        }
        #endregion

        if (product.MainPhoto != null)
        {
            IEnumerable<string> errors = product.MainPhoto.VerifyFile();
            if (errors.Any())
            {
                foreach (string error in errors) ModelState.AddModelError("Photo", error);
                return View(product);
            }
            string filename = await product.MainPhoto.SaveFileAsync();
            product.MainImage = filename;
        }

        if (product.Photos != null)
        {
            foreach (IFormFile file in product.Photos)
            {
                IEnumerable<string> fileErrors = product.MainPhoto.VerifyFile();
                if (fileErrors.Any())
                {
                    foreach (string error in fileErrors) ModelState.AddModelError("Photo", error);
                }
                return View(product);
            }
            List<ProductImage> images = new();
            foreach (IFormFile file in product.Photos)
            {
                string name = await file.SaveFileAsync();
                ProductImage image = new() { Image = name, ProductId = product.Id };
                images.Add(image);
            }
            product.ProductImages = images;
        }

        product.ProductColors = productColors;
        product.ProductSizes = productSizes;
        product.ProductCategories = productCategories;
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        foreach (ProductImage productImage in product.ProductImages) DeleteFile.Delete(productImage.Image, "product");
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }
}
