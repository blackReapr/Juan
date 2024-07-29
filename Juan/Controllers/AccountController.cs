using Juan.Data;
using Juan.Extensions;
using Juan.Helpers;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

[Authorize(Roles = "member")]
public class AccountController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(JuanDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return BadRequest();
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == appUser.Id).AsNoTracking().ToListAsync();
        ViewBag.Orders = orders;
        AccountVM accountVM = new()
        {
            Email = appUser.Email,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            UserName = appUser.UserName

        };
        return View(accountVM);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Index(AccountVM accountVM)
    {
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return BadRequest();
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == appUser.Id).AsNoTracking().ToListAsync();
        ViewBag.Orders = orders;
        if (!ModelState.IsValid) return View(accountVM);

        IEnumerable<string> errors = accountVM.Profile.VerifyFile();
        if (errors.Any())
        {
            foreach (string error in errors) ModelState.AddModelError("Profile", error);
            return View(accountVM);
        }

        string filename = await accountVM.Profile.SaveFileAsync();
        string? oldFile = appUser.Profile == "default.jpg" ? null : appUser.Profile;

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(appUser, accountVM.CurrentPassword);
        if (!isPasswordCorrect)
        {
            ModelState.AddModelError("Error", "Incorrect Password");
            return View(accountVM);
        }

        if (accountVM.Password != null)
        {
            IdentityResult result = await _userManager.ChangePasswordAsync(appUser, accountVM.CurrentPassword, accountVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("Error", error.Description);
                return View(accountVM);
            }
        }

        appUser.FirstName = accountVM.FirstName;
        appUser.LastName = accountVM.LastName;
        appUser.Email = accountVM.Email;
        appUser.UserName = accountVM.UserName;
        appUser.Profile = filename;

        IdentityResult updateResult = await _userManager.UpdateAsync(appUser);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors) ModelState.AddModelError("Error", error.Description);
            return View(accountVM);
        }

        if (oldFile != null) DeleteFile.Delete(filename, "users");

        await _signInManager.SignInAsync(appUser, true);
        return View();
    }
}
