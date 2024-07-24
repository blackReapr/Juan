using Juan.Models;

namespace Juan.ViewModels;

public class HomeVM
{
    public IEnumerable<Slider> Sliders { get; set; }
    public IEnumerable<Product> Products { get; set; }
    public IEnumerable<Product> NewProducts { get; set; }
    public IEnumerable<Blog> Blogs { get; set; }
}
