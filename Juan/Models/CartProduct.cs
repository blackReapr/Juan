namespace Juan.Models;

public class CartProduct : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
}
