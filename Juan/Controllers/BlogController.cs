using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

[Authorize(Roles = "memeber")]
public class BlogController : Controller
{
    private readonly JuanDbContext _context;

    public BlogController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Detail(int? id)
    {
        Blog? blog = await _context.Blogs.Include(b => b.BlogTags).ThenInclude(bt => bt.Tag).Include(b => b.User).AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        if (blog == null) return NotFound();
        IEnumerable<Blog> blogs = await _context.Blogs.AsNoTracking().OrderByDescending(b => b.CreatedAt).Take(4).ToListAsync();
        BlogVM vm = new()
        {
            Blog = blog,
            Blogs = blogs
        };
        return View(vm);
    }
}
