using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class ProductImage : BaseEntity
{
    public string Image { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
}
