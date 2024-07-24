using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.Win32;

namespace Juan.Controllers;

public class AuthenticationController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public AuthenticationController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Register()
    {
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
                ModelState.AddModelError("", error.Description);
            }
            return View(registerVM);
        }
        //await _userManager.AddToRoleAsync(user, "member");

        return RedirectToAction(nameof(Login));
    }
    public IActionResult Login()
    {
        return View();
    }
}
