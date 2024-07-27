using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class OrderItem : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    [Required, Column(TypeName = "money")]
    public decimal Price { get; set; }
    public int Count { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
}
