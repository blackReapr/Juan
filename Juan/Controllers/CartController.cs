using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

[Authorize(Roles = "memeber")]
public class CartController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CartController(JuanDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        CartPartialVM? cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]) ?? new();
        return View(cart);
    }

    public async Task<IActionResult> CartIndexPartial()
    {
        CartPartialVM? cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]) ?? new();
        return PartialView("_CartIndexPartial", cart);
    }

    public async Task<IActionResult> AddToCart(int? id)
    {
        if (id == null) return BadRequest();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return BadRequest();
        CartPartialVM cart;
        string basket = HttpContext.Request.Cookies["cart"];
        if (string.IsNullOrEmpty(basket)) cart = new();

        else
        {
            cart = CartPartialVMConverter.Deserialize(basket);
            if (cart.Exists(p => p.Id == id))
            {
                cart.Find(p => p.Id == id).Count++;
            }
            else
            {
                cart.Add(new()
                {
                    Id = product.Id,
                    Count = 1,
                    Image = product.MainImage,
                    Name = product.Name,
                    Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price
                });
            }
        }
        if (User.Identity.IsAuthenticated)
        {
            AppUser? user = await _userManager.Users.Include(u => u.CartProducts).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (user == null) return BadRequest();
            if (user.CartProducts.Any(b => b.ProductId == id))
                user.CartProducts.Find(b => b.ProductId == id).Count++;
            else
            {
                CartProduct newBasket = new()
                {
                    ProductId = product.Id,
                    UserId = user.Id,
                    Count = 1
                };
                user.CartProducts.Add(newBasket);
            }
            await _context.SaveChangesAsync();
        }
        HttpContext.Response.Cookies.Append("cart", CartPartialVMConverter.Serialize(cart));

        return PartialView("_CartPartial", cart);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveProduct(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        CartPartialVM? cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]);
        if (cart == null) return NotFound();
        CartVM? existProduct = cart.FirstOrDefault(p => p.Id == product.Id);
        if (existProduct == null) return NotFound();
        cart.Remove(existProduct);
        HttpContext.Response.Cookies.Append("cart", CartPartialVMConverter.Serialize(cart));
        if (User.Identity.IsAuthenticated)
        {
            var cartProduct = await _context.CartProducts.Include(p => p.User).FirstOrDefaultAsync(p => p.ProductId == id && p.User.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (cartProduct == null) return BadRequest();
            _context.CartProducts.Remove(cartProduct);
            await _context.SaveChangesAsync();
        }
        return PartialView("_CartPartial", cart);
    }

    public async Task<IActionResult> Decrement(int? id)
    {
        if (id == null) return BadRequest();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return BadRequest();
        CartPartialVM cart;
        string basket = HttpContext.Request.Cookies["cart"];
        if (string.IsNullOrEmpty(basket)) cart = new();
        else
        {
            cart = CartPartialVMConverter.Deserialize(basket);
            if (cart.Exists(p => p.Id == id))
            {
                var existCart = cart.Find(p => p.Id == id);
                if (existCart.Count > 1) existCart.Count--;
                else cart.Remove(existCart);
            }
        }
        if (User.Identity.IsAuthenticated)
        {
            AppUser? user = await _userManager.Users.Include(u => u.CartProducts).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (user == null) return BadRequest();
            if (user.CartProducts.Any(b => b.ProductId == id))
            {
                var existCart = user.CartProducts.Find(b => b.ProductId == id);
                if (existCart.Count > 1) existCart.Count--;
                else user.CartProducts.Remove(existCart);
                await _context.SaveChangesAsync();
            }
            else
            {
                CartProduct newBasket = new()
                {
                    ProductId = product.Id,
                    UserId = user.Id,
                    Count = 1
                };
                user.CartProducts.Add(newBasket);
            }
            await _context.SaveChangesAsync();
        }
        HttpContext.Response.Cookies.Append("cart", CartPartialVMConverter.Serialize(cart));

        return PartialView("_CartPartial", cart);
    }

    public async Task<IActionResult> Coupon(string? coupon)
    {
        CartPartialVM? cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]) ?? new();
        IDictionary<string, string> data = new Dictionary<string, string>();
        Coupon? existingCoupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Name == coupon);

        if (User.Identity.IsAuthenticated)
        {
            AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (appUser == null) return BadRequest();
            if (appUser.CouponId == null) appUser.CouponId = existingCoupon?.Id;
            else
            {
                Response.StatusCode = 400;
                data["error"] = "You already have an active coupon";
                return Json(data);
            }

            await _context.SaveChangesAsync();
        }

        if (coupon == null || existingCoupon == null)
        {
            Response.StatusCode = 400;
            data["error"] = "Invalid coupon code";
            return Json(data);
        }

        cart.Coupon = existingCoupon.Name;
        cart.DiscountRate = existingCoupon.Sale;



        Response.Cookies.Append("cart", CartPartialVMConverter.Serialize(cart));

        return PartialView("_CartIndexPartial", cart);
    }
}
