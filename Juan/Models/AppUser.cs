using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Juan.Models;

public class AppUser : IdentityUser
{
    public bool IsBanned { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Profile { get; set; } = "default.jpg";
    public string? Profession { get; set; }
    public string? Facebook { get; set; }
    public string? Vimeo { get; set; }
    public string? Twitter { get; set; }
    public string? Pinterest { get; set; }
    [NotMapped]
    public IFormFile? Image { get; set; }
    public List<CartProduct> CartProducts { get; set; }
    public List<Review> Reviews { get; set; }
    public List<Wishlist> Wishlists { get; set; }
    public List<Order> Orders { get; set; }
    public List<Announcement> Annoucements { get; set; }
    [AllowNull]
    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    public string? ClientId { get; set; }
}
