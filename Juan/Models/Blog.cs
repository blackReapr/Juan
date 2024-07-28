using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Juan.Models;

public class Blog : BaseEntity
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public string? Image { get; set; }
    [NotMapped]
    public IFormFile? Photo { get; set; }
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    public List<BlogTag>? BlogTags { get; set; }
    [NotMapped]
    public List<int>? TagIds { get; set; }
}
