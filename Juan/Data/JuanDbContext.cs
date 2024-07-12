using Juan.Models;
using Microsoft.EntityFrameworkCore;

namespace Juan.Data;

public class JuanDbContext : DbContext
{
    public DbSet<Slider> Sliders { get; set; }
    public JuanDbContext(DbContextOptions options) : base(options) { }
}
