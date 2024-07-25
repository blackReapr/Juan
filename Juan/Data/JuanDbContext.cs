using Juan.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Juan.Data;

public class JuanDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }
    public JuanDbContext(DbContextOptions options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new Interceptor());
        base.OnConfiguring(optionsBuilder);
    }
}
