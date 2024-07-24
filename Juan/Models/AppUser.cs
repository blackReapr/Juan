using Microsoft.AspNetCore.Identity;

namespace Juan.Models;

public class AppUser : IdentityUser
{
    public bool IsBanned { get; set; }
}
