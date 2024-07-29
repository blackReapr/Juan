using System.ComponentModel.DataAnnotations;

namespace Juan.ViewModels;

public class AccountVM
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required, DataType(DataType.EmailAddress), EmailAddress]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [DataType(DataType.Password), Compare(nameof(Password))]
    public string RePassword { get; set; }
    [Required, DataType(DataType.Password)]
    public string CurrentPassword { get; set; }
    public IFormFile? Profile { get; set; }
}
