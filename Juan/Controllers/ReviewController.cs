using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class ReviewController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ReviewController(JuanDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Reviews(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        IEnumerable<Review> reviews = await _context.Reviews.AsNoTracking().Include(r => r.User).Where(r => r.ProductId == product.Id).ToListAsync();
        return PartialView("_ReviewPartial", reviews);
    }

    [HttpPost]
    public async Task<IActionResult> New([FromBody] ReviewVM reviewVM, int? id)
    {
        string content = reviewVM.Content;
        int rating = reviewVM.Rating;
        if (id == null) return BadRequest();
        Product? product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        IDictionary<string, string> errors = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(content)) errors["content"] = "Content is required";
        if (rating <= 0 || rating > 5) errors["rating"] = "Invalid rating value";

        if (!User.Identity.IsAuthenticated) errors["user"] = "You must be authenticated to post a review";

        AppUser? user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null) errors["user"] = "You must be authenticated to post a review";

        if (errors.Count > 0)
        {
            Response.StatusCode = 400;
            return Json(errors);
        }
        Review review = new();
        review.UserId = user.Id;
        review.ProductId = product.Id;
        review.Rating = rating;
        review.Content = content;
        _context.Reviews.Add(review);
        List<Review> reviews = await _context.Reviews.Include(r => r.User).Where(r => r.ProductId == product.Id).ToListAsync();
        reviews.Add(review);
        await _context.SaveChangesAsync();
        return PartialView("_ReviewPartial", reviews);
    }
}
