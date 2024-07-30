using Juan.Data;
using Juan.Helpers;
using Juan.Interfaces;
using Juan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Juan.Areas.Admin.Controllers;

[Area("Admin")]
public class AuthenticationController : DefaultAuthentication
{
    public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JuanDbContext context, string role, IEmailService emailService) : base(userManager, signInManager, context, role, emailService)
    {
    }
}
