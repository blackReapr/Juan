using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Identity;


namespace Juan.Controllers;

public class AuthenticationController : DefaultAuthentication
{
    public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JuanDbContext context) : base(userManager, signInManager, context, "member") { }
}
