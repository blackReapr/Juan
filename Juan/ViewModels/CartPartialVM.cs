namespace Juan.ViewModels;

public class CartPartialVM : List<CartVM>
{
    public string? Coupon { get; set; }
    public decimal DiscountRate { get; set; }
}
