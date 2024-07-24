using Microsoft.AspNetCore.Identity;

namespace Juan.Models;

public class AppUser : IdentityUser
{
    public string Username { get; set; }
    public bool IsBanned { get; set; }
}
