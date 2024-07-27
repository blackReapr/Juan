using Juan.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class Order : BaseEntity
{
    [Required]
    public string UserId { get; set; }
    public AppUser User { get; set; }
    [Required]
    public string Country { get; set; }
    [Required]
    public string MainStreetAddress { get; set; }
    public string? SecondaryStreetAddress { get; set; }
    [Required]
    public string TownOrCity { get; set; }
    public string? StateOrDivision { get; set; }
    public string? CompanyName { get; set; }
    public string Zip { get; set; }
    public string? Phone { get; set; }
    public string? OrderNote { get; set; }
    [Required]
    public PaymentEnum PaymentMethod { get; set; }
    public ShippingEnum Shipping { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    [Column(TypeName = "money")]
    public decimal DiscountRate { get; set; }
    public StatusEnum Status { get; set; } = StatusEnum.Pending;
}
