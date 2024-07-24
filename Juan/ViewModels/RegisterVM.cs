using System.ComponentModel.DataAnnotations;

namespace Juan.ViewModels;

public class RegisterVM
{
    [Required]
    public string Username { get; set; }
    [EmailAddress, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    public string RePassword { get; set; }
    public bool Subscribe { get; set; }
}
