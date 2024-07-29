using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Juan.Helpers;

public class DefaultAuthentication : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JuanDbContext _context;
    private readonly string _role;

    public DefaultAuthentication(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JuanDbContext context, string role)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _role = role;
    }

    public IActionResult Register() => View();

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid) return View(registerVM);
        AppUser user = new() { Email = registerVM.Email, UserName = registerVM.Username };
        IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Error", error.Description);
            }
            return View(registerVM);
        }
        await _userManager.AddToRoleAsync(user, _role);

        if (registerVM.Subscribe)
        {
            Subscribe subscribe = new()
            {
                UserId = user.Id,
            };

            await _context.Subscribes.AddAsync(subscribe);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Login));
    }
    public IActionResult Login() => View();

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
    {
        if (!ModelState.IsValid) return View(loginVM);
        AppUser? user = await _userManager.Users.Include(u => u.CartProducts).ThenInclude(cp => cp.Product).FirstOrDefaultAsync(u => u.NormalizedUserName == loginVM.UsernameOrEmail.ToUpperInvariant() || u.NormalizedEmail == loginVM.UsernameOrEmail.ToUpperInvariant());
        if (user == null)
        {
            ModelState.AddModelError("Error", "Invalid username/email or password");
            return View(loginVM);
        }

        bool doesPasswordMatch = await _userManager.CheckPasswordAsync(user, loginVM.Password);
        if (!doesPasswordMatch)
        {
            ModelState.AddModelError("Error", "Username or password is wrong.");
            return View(loginVM);
        }
        if (user.IsBanned)
        {
            ModelState.AddModelError("Error", "Account is banned.");
            return View(loginVM);
        }
        var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
        if (result.IsLockedOut)
        {
            ModelState.AddModelError("Error", "User is locked out. Try again later.");
            return View(loginVM);
        }
        if (!result.Succeeded)
        {
            ModelState.AddModelError("Error", "Verify email.");
            return View(loginVM);
        }

        // Get cart if user is memeber
        if (_role == "member")
        {
            Response.Cookies.Append("cart", "");
            if (user.CartProducts != null && user.CartProducts.Count() > 0)
            {
                List<CartVM> cart = new();
                foreach (var basket in user.CartProducts)
                {
                    CartVM cartVM = new();
                    cartVM.Id = basket.ProductId;
                    cartVM.Price = basket.Product.DiscountPrice > 0 ? basket.Product.DiscountPrice : basket.Product.Price;
                    cartVM.Count = basket.Count;
                    cartVM.Image = basket.Product.MainImage;
                    cartVM.Name = basket.Product.Name;
                    cart.Add(cartVM);
                }
                Response.Cookies.Append("cart", JsonConvert.SerializeObject(cart));
            }
        }

        if (returnUrl is null)
            return RedirectToAction("Index", "Home");
        return Redirect(returnUrl);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        if (_role == "member")
        {
            Response.Cookies.Append("cart", "");
            return RedirectToAction("index", "home");
        }
        return RedirectToAction("login", "authentication", "admin");
    }
}
