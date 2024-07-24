using System.ComponentModel.DataAnnotations;

namespace Juan.ViewModels;

public class LoginVM
{
    [Required, Display(Name = "Username or Email")]
    public string UsernameOrEmail { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "Remember Me?")]
    public bool RememberMe { get; set; }
}
