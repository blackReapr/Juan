using Juan.Data;
using Juan.Data.Migrations;
using Juan.Models;
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
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        IEnumerable<Review> reviews = await _context.Reviews.Include(r => r.User).Where(r => r.ProductId == product.Id).ToListAsync();
        return PartialView("_ReviewPartial", reviews);
    }

    [HttpPost]
    public async Task<IActionResult> NewReview(Review review, int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        if (!ModelState.IsValid) return View(review);
        if (!User.Identity.IsAuthenticated)
        {
            ModelState.AddModelError("Error", "You must be authenticated to post a review.");
            return View(review);
        }
        AppUser? user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null)
        {
            ModelState.AddModelError("Error", "You must be authenticated to post a review.");
            return View(review);
        }
        review.UserId = user.Id;
        review.ProductId = product.Id;
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        IEnumerable<Review> reviews = await _context.Reviews.Include(r => r.User).Where(r => r.ProductId == product.Id).ToListAsync();
        return PartialView("_ReviewPartial", reviews);
    }
}
