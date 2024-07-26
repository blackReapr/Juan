using Juan.Models;

namespace Juan.ViewModels;

public class ShopVM
{
    public IEnumerable<Category> Categories { get; set; }
    public PaginationVM<Product> Products { get; set; }
    public IEnumerable<Color> Colors { get; set; }
    public IEnumerable<Size> Sizes { get; set; }
    public IEnumerable<IDictionary<string, string>> ProductCategories { get; set; }
    public IEnumerable<IDictionary<string, string>> ProductSizes { get; set; }
    public IEnumerable<IDictionary<string, string>> ProductColors { get; set; }
}
