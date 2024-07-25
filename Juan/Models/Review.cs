using System.ComponentModel.DataAnnotations;

namespace Juan.Models;

public class Review : BaseEntity
{
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    [Required, MaxLength(200), Display(Name = "Your review")]
    public string Content { get; set; }
    [Required, Range(1, 5)]
    public int Raiting { get; set; }
}
