using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Juan.Controllers;

public class AuthenticationController : DefaultAuthentication
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JuanDbContext context) : base(userManager, signInManager, context, "member")
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> SigninGoogle()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), null);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return new ChallengeResult("Google", properties);
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        string? email = info.Principal.FindFirstValue(ClaimTypes.Email);
        string? givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        string? phone = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);
        string? surname = info.Principal.FindFirstValue(ClaimTypes.Surname);


        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
        if (result.Succeeded)
        {
            // If the user already has a login, redirect to the home page.
            await _signInManager.SignInAsync(await _userManager.FindByEmailAsync(email), true);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // If the user does not have an account, we need to create one.
            var user = new AppUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phone,
                FirstName = givenName,
                LastName = surname
            };
            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(user, info);
                await _userManager.AddToRoleAsync(user, "member");
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }
        }


        return RedirectToAction("index", "home");
    }

    public IActionResult SetUsername(GoogleAuthVM googleAuthVM)
    {
        return View(googleAuthVM);
    }
}
