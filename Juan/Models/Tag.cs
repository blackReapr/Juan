using Juan.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Tag : BaseEntity, IDefaultEntity
{
    [Required]
    public string Name { get; set; }
    public List<BlogTag>? BlogTags { get; set; }
}
