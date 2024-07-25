using Juan.Models;

namespace Juan.ViewModels;

public class ShopVM
{
    public IEnumerable<Category> Categories { get; set; }
    public IEnumerable<Product> Products { get; set; }
    public IEnumerable<Color> Colors { get; set; }
    public IEnumerable<Size> Sizes { get; set; }
    public IEnumerable<ProductSize> ProductSizes { get; set; }
    public IEnumerable<ProductCategory> ProductCategories { get; set; }
    public IEnumerable<ProductColor> ProductColors { get; set; }
}
