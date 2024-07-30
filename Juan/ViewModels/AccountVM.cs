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
    public IFormFile? Profile { get; set; }
}
