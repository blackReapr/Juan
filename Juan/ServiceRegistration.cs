using Juan.Data;
using Juan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Juan;

public static class ServiceRegistration
{
    public static void Register(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<JuanDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));
        services.AddControllersWithViews();
        services.AddSession();
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            //options.SignIn.RequireConfirmedEmail = true;
        }).AddEntityFrameworkStores<JuanDbContext>().AddDefaultTokenProviders();
    }
}
