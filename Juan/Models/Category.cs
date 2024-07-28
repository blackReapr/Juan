using Juan.Interfaces;

namespace Juan.Models;

public class Category : BaseEntity, IDefaultEntity
{
    public string Name { get; set; }
    public List<ProductCategory>? ProductCategories { get; set; }
}
