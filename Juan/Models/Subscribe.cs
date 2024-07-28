namespace Juan.Models;

public class Subscribe : BaseEntity
{
    public string UserId { get; set; }
    public AppUser User { get; set; }
}
