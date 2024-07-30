using Juan.Data;
using Juan.Interfaces;
using Juan.Models;
using Juan.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Juan;

public static class ServiceRegistration
{
    public static void Register(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<JuanDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddSession();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = configuration["Google:ClientId"];
            options.ClientSecret = configuration["Google:ClientSecret"];
        });
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<JuanDbContext>()
        .AddDefaultTokenProviders();


        services.AddControllersWithViews();
        services.AddSignalR();

        services.AddSingleton<Interceptor>();
        services.AddHttpContextAccessor();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ILayoutService, LayoutService>();
        services.AddHttpClient<IZipCodeValidataionService, ZipCodeValidataionService>();
    }
}
