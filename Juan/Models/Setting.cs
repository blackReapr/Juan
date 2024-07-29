using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Setting : BaseEntity
{
    [Required]
    public string Key { get; set; }
    [Required]
    public string Value { get; set; }
}
