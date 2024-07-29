using Juan.Data;
using Juan.Helpers;
using Juan.Interfaces;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

[Authorize(Roles = "memeber"), AllowAnonymous]
public class CheckoutController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IZipCodeValidataionService _zipCodeValidataionService;

    public CheckoutController(JuanDbContext context, UserManager<AppUser> userManager, IZipCodeValidataionService zipCodeValidataionService)
    {
        _context = context;
        _userManager = userManager;
        _zipCodeValidataionService = zipCodeValidataionService;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated) return RedirectToAction("login", "authentication");
        CartPartialVM cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]) ?? new();
        ViewBag.Cart = cart;
        return View();
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Index(CheckoutVM checkoutVM)
    {
        if (!User.Identity.IsAuthenticated) return RedirectToAction("login", "authentication");
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return RedirectToAction("login", "authentication");
        CartPartialVM cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]) ?? new();
        ViewBag.Cart = cart;
        if (!ModelState.IsValid) return View(checkoutVM);

        bool isValidZip = await _zipCodeValidataionService.ValidateZipCodeAsync(checkoutVM.Zip);
        if (!isValidZip)
        {
            ModelState.AddModelError("Zip", "The ZIP code is invalid.");
            return View(checkoutVM);
        }

        Order order = new();
        order.OrderItems = new();
        foreach (CartVM cartVM in cart)
        {
            OrderItem orderItem = new();
            orderItem.ProductId = cartVM.Id;
            orderItem.Count = cartVM.Count;
            orderItem.Price = cartVM.Price;
            orderItem.OrderId = order.Id;
            order.OrderItems.Add(orderItem);
        }
        Coupon? coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Name == cart.Coupon);
        if (coupon != null)
        {
            order.CouponId = coupon.Id;
            order.DiscountRate = coupon.Sale;
        }
        order.UserId = appUser.Id;
        order.Zip = checkoutVM.Zip;
        order.CompanyName = checkoutVM.CompanyName;
        order.Country = checkoutVM.Country;
        order.Shipping = checkoutVM.Shipping;
        order.MainStreetAddress = checkoutVM.MainStreetAddress;
        order.SecondaryStreetAddress = checkoutVM.SecondaryStreetAddress;
        order.StateOrDivision = checkoutVM.StateOrDivision;
        order.OrderNote = checkoutVM.OrderNote;
        order.PaymentMethod = checkoutVM.PaymentMethod;
        order.Phone = checkoutVM.Phone;
        order.TownOrCity = checkoutVM.TownOrCity;
        await _context.Orders.AddAsync(order);
        appUser.CouponId = null;
        IEnumerable<CartProduct> currentCartProducts = await _context.CartProducts.Where(cp => cp.UserId == appUser.Id).ToListAsync();
        _context.CartProducts.RemoveRange(currentCartProducts);
        await _userManager.UpdateAsync(appUser);
        await _context.SaveChangesAsync();
        return RedirectToAction("index", "home");
    }

    public IActionResult CheckoutPartial()
    {
        CartPartialVM cart = CartPartialVMConverter.Deserialize(Request.Cookies["cart"]) ?? new();
        return PartialView("_CheckoutPartial", cart);
    }
}
