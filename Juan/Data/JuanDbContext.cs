using Juan.Models;
using Microsoft.EntityFrameworkCore;

namespace Juan.Data;

public class JuanDbContext : DbContext
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public JuanDbContext(DbContextOptions options) : base(options) { }
}
