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

    [HttpPost, AutoValidateAntiforgeryToken]
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
        Slider newSlider = new()
        {
            Title = slider.Title,
            SubTitle = slider.SubTitle,
            Description = slider.Description,
        };
        newSlider.Image = await slider.Photo.SaveAsync();
        newSlider.CreatedAt = DateTime.Now;
        _juanDbContext.Sliders.Add(newSlider);
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

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        Slider? slider = await _juanDbContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider == null) return NotFound();
        _juanDbContext.Sliders.Remove(slider);
        await _juanDbContext.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null) return BadRequest();
        Slider? slider = await _juanDbContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider == null) return NotFound();
        return View(slider);
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(int? id, Slider slider)
    {
        if (id == null) return BadRequest();
        if (slider.Id != id) return BadRequest();
        if (!ModelState.IsValid) return View(slider);
        Slider? existSlider = await _juanDbContext.Sliders.FirstOrDefaultAsync(s => s.Id == slider.Id);
        if (existSlider== null) return NotFound();
        existSlider.Title = slider.Title;
        existSlider.Description = slider.Description;
        existSlider.SubTitle = slider.SubTitle;
        if (slider.Photo is not null)
        {
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
            var oldImage = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", existSlider.Image);
            if (System.IO.File.Exists(oldImage))
                System.IO.File.Delete(oldImage);
            
            existSlider.Image = await slider.Photo.SaveAsync();
        }
        existSlider.UpdatedAt = DateTime.Now;
        await _juanDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
