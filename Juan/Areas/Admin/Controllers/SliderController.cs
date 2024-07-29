using Juan.Data;
using Juan.Extensions;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.Admin.Controllers;

[Area("admin"), Authorize(Roles = "admin")]
public class SliderController : Controller
{
    private readonly JuanDbContext _context;

    public SliderController(JuanDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Slider> sliders = await _context.Sliders.AsNoTracking().ToListAsync();
        return View(sliders);
    }

    public IActionResult Create() => View();

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(Slider slider)
    {
        if (!ModelState.IsValid) return View(slider);
        IEnumerable<string> errors = slider.Photo.VerifyFile();
        if (errors.Any())
        {
            foreach (string error in errors) ModelState.AddModelError("Photo", error);
            return View(slider);
        }
        string filename = await slider.Photo.SaveFileAsync("slider");
        slider.Image = filename;
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider == null) return BadRequest();
        return View(slider);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(int? id, Slider slider)
    {
        if (id == null) return BadRequest();
        Slider? existingSlider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (existingSlider == null) return NotFound();

        if (!ModelState.IsValid) return View(slider);

        if (slider.Photo != null)
        {
            IEnumerable<string> errors = slider.Photo.VerifyFile();
            if (errors.Any())
            {
                foreach (string error in errors) ModelState.AddModelError("Photo", error);
                return View(slider);
            }

            DeleteFile.Delete(existingSlider.Image, "slider");

            string filename = await slider.Photo.SaveFileAsync("slider");
            existingSlider.Image = filename;
        }

        existingSlider.Title = slider.Title;
        existingSlider.SubTitle = slider.SubTitle;
        existingSlider.Description = slider.Description;

        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        Slider? slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
        if (slider == null) return NotFound();
        DeleteFile.Delete(slider.Image, "slider");
        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }
}
