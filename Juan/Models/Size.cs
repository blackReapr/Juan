using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Size : BaseEntity
{
    [Required, MaxLength(5)]
    public string Name { get; set; }
    public List<ProductSize> productSizes;
}
