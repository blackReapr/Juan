using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Juan.Controllers;

public class WishlistController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public WishlistController(JuanDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        IEnumerable<WishlistVM>? wishlist = JsonSerializer.Deserialize<IEnumerable<WishlistVM>>(Request.Cookies["wishlist"]);
        if (wishlist == null) wishlist = new List<WishlistVM>();
        return View(wishlist);
    }

    [HttpPost]
    public async Task<IActionResult> New(int? id)
    {
        if (id == null) return BadRequest();
        List<WishlistVM>? wishlistCookie = new();
        if (Request.Cookies["wishlist"] != null) wishlistCookie = JsonSerializer.Deserialize<List<WishlistVM>>(Request.Cookies["wishlist"]);
        IDictionary<string, string> data = new Dictionary<string, string>();


        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        WishlistVM? existingWishlistVM = wishlistCookie.FirstOrDefault(w => w.Id == id);
        if (existingWishlistVM == null)
        {
            WishlistVM wishlistVM = new WishlistVM()
            {
                Id = product.Id,
                Image = product.MainImage,
                Name = product.Name,
                IsInStock = product.Count > 0,
                Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price,
            };
            wishlistCookie.Add(wishlistVM);
            data["message"] = "Product added to wishlist!";

            if (User.Identity.IsAuthenticated)
            {
                AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (appUser == null) return BadRequest();
                Wishlist wishlist = new Wishlist()
                {
                    UserId = appUser.Id,
                    ProductId = product.Id,
                };
                _context.Wishlists.Add(wishlist);
            }

        }
        else
        {
            wishlistCookie.Remove(existingWishlistVM);
            data["message"] = "Product removed from your wishlist!";
            Wishlist? wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Id != existingWishlistVM.Id);
            if (wishlist != null) _context.Wishlists.Remove(wishlist);
        }
        Response.Cookies.Append("wishlist", JsonSerializer.Serialize(wishlistCookie));
        await _context.SaveChangesAsync();

        return Json(data);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveProduct(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        var wishlistCookie = JsonSerializer.Deserialize<List<WishlistVM>>(Request.Cookies["wishlist"]);

        if (wishlistCookie != null)
        {
            var existingCookie = wishlistCookie.FirstOrDefault(w => w.Id == product.Id);
            if (existingCookie != null) wishlistCookie.Remove(existingCookie);
        }
        Response.Cookies.Append("wishlist", JsonSerializer.Serialize(wishlistCookie));

        if (User.Identity.IsAuthenticated)
        {
            AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (appUser != null)
            {
                var existingWishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == appUser.Id);
                if (existingWishlist != null)
                {
                    _context.Wishlists.Remove(existingWishlist);
                    await _context.SaveChangesAsync();
                }
            }
        }

        return PartialView("_WishlistPartial", wishlistCookie);
    }
}


