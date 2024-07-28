using Juan.Data;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.Admin.Controllers;

[Area("admin")]
public class SubscribeController : Controller
{
    private readonly JuanDbContext _context;

    public SubscribeController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Subscribe> subscribes = await _context.Subscribes.Include(s => s.User).AsNoTracking().ToListAsync();
        return View(subscribes);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        Subscribe? subscribe = await _context.Subscribes.FirstOrDefaultAsync(s => s.Id == id);
        if (subscribe == null) return NotFound();
        _context.Subscribes.Remove(subscribe);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
