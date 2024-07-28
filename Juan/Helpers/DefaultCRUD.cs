using Juan.Data;
using Juan.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Helpers;

public abstract class DefaultCRUD<T> : Controller where T : class, IDefaultEntity
{
    private readonly JuanDbContext _context;

    public DefaultCRUD(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<T> items = await _context.Set<T>().AsNoTracking().ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View();

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(T item)
    {
        if (!ModelState.IsValid) return View(item);
        T? existingItem = await _context.Set<T>().FirstOrDefaultAsync(i => i.Name.ToUpper() == item.Name.ToUpper());
        if (existingItem != null)
        {
            ModelState.AddModelError("Name", $"{nameof(T)} with that name already exists");
            return View(item);
        }
        await _context.Set<T>().AddAsync(item);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        T? item = await _context.Set<T>().FirstOrDefaultAsync(i => i.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(int? id, T item)
    {
        if (id == null || id != item.Id) return BadRequest();
        if (!ModelState.IsValid) return View(item);
        T? existingItem = await _context.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
        if (existingItem == null) return NotFound();
        if (await _context.Set<T>().AnyAsync(i => i.Name.ToUpper() == item.Name.ToUpper() && i.Id != id))
        {
            ModelState.AddModelError("Name", $"{nameof(T)} with that name already exists");
            return View(item);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        T? existingitem = await _context.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
        if (existingitem == null) return NotFound();
        _context.Set<T>().Remove(existingitem);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }
}
