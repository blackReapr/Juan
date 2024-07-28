using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Announcement : BaseEntity
{
    [Required]
    public string Title { get; set; }
    [Required, MaxLength(5000)]
    public string Content { get; set; }
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
}
