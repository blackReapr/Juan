using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class Slider : BaseEntity
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string SubTitle { get; set; }
    [Required]
    public string Description { get; set; }
    public string? Image { get; set; }
    [NotMapped]
    public IFormFile? Photo { get; set; }
}
