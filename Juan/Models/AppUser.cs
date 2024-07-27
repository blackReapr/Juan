using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Juan.Models;

public class AppUser : IdentityUser
{
    public bool IsBanned { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Profile { get; set; }
    [NotMapped]
    public IFormFile Image { get; set; }
    public List<CartProduct> CartProducts { get; set; }
    public List<Review> Reviews { get; set; }
    public List<Wishlist> Wishlists { get; set; }
    public List<Order> Orders { get; set; }
    [AllowNull]
    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
}
