using System.ComponentModel.DataAnnotations;

namespace Juan.ViewModels;

public class ResetPasswordVM
{
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    [DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm Password")]
    public required string RePassword { get; set; }
}
