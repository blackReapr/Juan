using Juan.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Color : BaseEntity, IDefaultEntity
{
    [Required, MaxLength(20)]
    public string Name { get; set; }
    public List<ProductColor> ProductColors { get; set; }
}
