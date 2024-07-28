using Juan.Models;

namespace Juan.ViewModels;

public class BlogVM
{
    public Blog Blog { get; set; }
    public IEnumerable<Blog> Blogs { get; set; }
}
