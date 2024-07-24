using Microsoft.AspNetCore.Identity;

namespace Juan.Models;

public class AppUser : IdentityUser
{
    public bool IsBanned { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<CartProduct> CartProducts { get; set; }
}
