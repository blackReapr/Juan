using Juan.DataAnnotations;
using Juan.Enums;
using System.ComponentModel.DataAnnotations;

namespace Juan.ViewModels;

public class CheckoutVM
{
    [Required]
    public string Country { get; set; }
    [MaxLength(300), Display(Name = "Order Note")]
    public string? OrderNote { get; set; }
    [Display(Name = "Town / City"), Required]
    public string TownOrCity { get; set; }
    [Display(Name = "State / Division")]
    public string? StateOrDivision { get; set; }
    [Display(Name = "Postcode / ZIP"), Required]
    public string Zip { get; set; }
    [Required, Display(Name = "Street Address")]
    public string MainStreetAddress { get; set; }
    public string? SecondaryStreetAddress { get; set; }
    [Display(Name = "Company Name")]
    public string? CompanyName { get; set; }
    [Phone]
    public string? Phone { get; set; }
    [Required]
    public PaymentEnum PaymentMethod { get; set; }
    [Required, MustBeTrue(ErrorMessage = "You must accept the terms and conditions.")]
    public bool Terms { get; set; }
    public ShippingEnum Shipping { get; set; }
}


