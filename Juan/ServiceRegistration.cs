using Juan.Data;
using Juan.Interfaces;
using Juan.Models;
using Juan.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Juan;

public static class ServiceRegistration
{
    public static void Register(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<Interceptor>();
        services.AddDbContext<JuanDbContext>((sp, options) => options
        .UseSqlServer(configuration
        .GetConnectionString("Default"))
        .AddInterceptors(sp.GetRequiredService<Interceptor>()));
        services.AddControllersWithViews();
        services.AddSession();
        services.AddScoped<ILayoutService, LayoutService>();
        services.AddHttpClient<IZipCodeValidataionService, ZipCodeValidataionService>();
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            //options.SignIn.RequireConfirmedEmail = true;
        }).AddEntityFrameworkStores<JuanDbContext>().AddDefaultTokenProviders();
    }
}
