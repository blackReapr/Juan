using Juan.Data;
using Juan.Interfaces;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;

namespace Juan.Helpers;

public class DefaultAuthentication : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JuanDbContext _context;
    private readonly IEmailService _emailService;
    private readonly string _role;

    public DefaultAuthentication(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JuanDbContext context, string role, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _role = role;
        _emailService = emailService;
    }

    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated) return RedirectToAction("index", "home", _role == "admin" ? "admin" : "");
        return View();
    }

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


        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string link = Url.Action(nameof(VerifyEmail), "Authentication", new { email = user.Email, token = token }, Request.Scheme, Request.Host.ToString());

        string body = "";
        using (StreamReader stream = new StreamReader("wwwroot/templates/verifyEmail.html"))
        {
            body = stream.ReadToEnd();
        };
        body = body.Replace("{{link}}", link);
        body = body.Replace("{{username}}", user.UserName);

        _emailService.SendEmail(user.Email, "Verify Email", body);



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

    public async Task<IActionResult> VerifyEmail(string token, string email)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null) return NotFound();
        await _userManager.ConfirmEmailAsync(appUser, token);
        return RedirectToAction(nameof(Login));
    }


    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated) return RedirectToAction("index", "home", _role == "admin" ? "admin" : "");
        return View();
    }

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



    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            ModelState.AddModelError("Error1", "User not found");
            return View();
        };

        string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
        string link = Url.Action(nameof(ResetPassword), "Authentication", new { email = appUser.Email, token = token }, Request.Scheme, Request.Host.ToString());

        string body = "";
        using (StreamReader stream = new StreamReader("wwwroot/templates/forgotPassword.html"))
        {
            body = stream.ReadToEnd();
        };
        body = body.Replace("{{link}}", link);
        body = body.Replace("{{username}}", appUser.UserName);

        _emailService.SendEmail(appUser.Email, "Reset Password", body);

        return View();
    }

    public async Task<IActionResult> ResetPassword(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return NotFound();
        bool result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
        if (!result) return BadRequest();
        return View();
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> ResetPassword(string email, string token, ResetPasswordVM resetPasswordVM)
    {
        AppUser? appUser = await _userManager.FindByEmailAsync(email);
        if (!ModelState.IsValid) return View();
        if (appUser == null)
        {
            ModelState.AddModelError("Error1", "User not found");
            return View(resetPasswordVM);
        };

        var result = await _userManager.ResetPasswordAsync(appUser, token, resetPasswordVM.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(resetPasswordVM);
        }
        await _userManager.UpdateSecurityStampAsync(appUser);

        return RedirectToAction("login");
    }

}
