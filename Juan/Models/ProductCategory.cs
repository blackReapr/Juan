namespace Juan.Models;

public class ProductCategory : BaseEntity
{
    public string UserId { get; set; }
    public AppUser? User { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
