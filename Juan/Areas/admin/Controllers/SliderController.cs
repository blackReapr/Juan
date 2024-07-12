using Juan.Data;
using Juan.Extensions;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.admin.Controllers;

[Area("admin")]
public class SliderController : Controller
{
    private readonly JuanDbContext _juanDbContext;

    public SliderController(JuanDbContext juanDbContext)
    {
        _juanDbContext = juanDbContext;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Slider> sliders = await _juanDbContext.Sliders.ToListAsync();
        return View(sliders);
    }
    public IActionResult Create()
    {
        return View();
    }

    public async Task<IActionResult> Create(Slider slider)
    {
        if (!ModelState.IsValid) return View(slider);
        if (slider.Photo == null)
        {
            ModelState.AddModelError("Photo", "Photo is require");
            return View(slider);
        }
        if (!slider.Photo.IsImage())
        {
            ModelState.AddModelError("Photo", "Invalid format");
            return View(slider);
        }
        if (slider.Photo.DoesExceed(100))
        {
            ModelState.AddModelError("Photo", "Size exceeded");
            return View(slider);
        }
        slider.Image = await slider.Photo.SaveAsync();
        await _juanDbContext.SaveChangesAsync();
        
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        Slider? slider = await _juanDbContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider == null) return NotFound();
        return View(slider);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(int? id, Slider slider)
    {
        if (id == null || id != slider.Id) return BadRequest();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        Slider? slider = await _juanDbContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider == null) return NotFound();
        _juanDbContext.Sliders.Remove(slider);
        await _juanDbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
