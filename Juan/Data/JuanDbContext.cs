using Juan.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;

namespace Juan.Data;

public class JuanDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<ProductColor> ProductColors { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Subscribe> Subscribes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BlogTag> BlogTags { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public JuanDbContext(DbContextOptions options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new Interceptor());
        base.OnConfiguring(optionsBuilder);
    }
}
