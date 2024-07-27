using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class Coupon : BaseEntity
{
    [Required]
    public string Name { get; set; }
    [Required, Column(TypeName = "money")]
    public decimal Sale { get; set; }
    public List<AppUser>? Users { get; set; }
    public List<Order>? Orders { get; set; }
}
