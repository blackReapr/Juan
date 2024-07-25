using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class Product : BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Column(TypeName = "money")]
    public decimal Price { get; set; }
    [Column(TypeName = "money")]
    public decimal DiscountPrice { get; set; }
    public int Count { get; set; }
    public string? MainImage { get; set; }
    public bool IsNew { get; set; }
    [MaxLength(5000)]
    public string DetailedDescription { get; set; }
    public List<ProductImage>? ProductImages { get; set; }
    [NotMapped]
    public IFormFile? MainPhoto { get; set; }
    [NotMapped]
    public IFormFile[]? Photos { get; set; }
    public List<ProductSize> ProductSizes { get; set; }
    public List<ProductColor> ProductColors { get; set; }
    public List<Review> Reviews { get; set; }
    public List<ProductCategory> ProductCategories { get; set; }
}