using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Tag : BaseEntity
{
    [Required]
    public string Name { get; set; }
    public List<BlogTag>? BlogTags { get; set; }
}
