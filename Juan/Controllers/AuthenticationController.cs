using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Juan.Controllers;

public class AuthenticationController : DefaultAuthentication
{
    public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, JuanDbContext context) : base(userManager, signInManager, context, "member") { }
}
