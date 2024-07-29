using Juan.Data;
using Juan.Interfaces;
using Juan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.Admin.Controllers;

[Area("admin"), Authorize(Roles = "admin")]
public class AnnouncementController : Controller
{
    private readonly JuanDbContext _context;
    private readonly IEmailService _emailService;
    private readonly UserManager<AppUser> _userManager;

    public AnnouncementController(JuanDbContext context, IEmailService emailService, UserManager<AppUser> userManager)
    {
        _context = context;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Announcement> annoucements = await _context.Announcements.Include(a => a.User).AsNoTracking().ToListAsync();
        return View(annoucements);
    }

    public IActionResult Create() => View();

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(Announcement announcement)
    {
        if (!ModelState.IsValid) return View(announcement);

        string body = "";
        using (StreamReader stream = new StreamReader("wwwroot/templates/announcement.html"))
        {
            body = stream.ReadToEnd();
        };
        body = body.Replace("{{title}}", announcement.Title);
        body = body.Replace("{{content}}", announcement.Content);

        IEnumerable<string> emails = await _context.Subscribes.Select(s => s.User.Email).AsNoTracking().ToListAsync();

        foreach (string email in emails) _emailService.SendEmail(email, "New Annoucement", body);

        announcement.UserId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id;
        await _context.Announcements.AddAsync(announcement);
        await _context.SaveChangesAsync();

        return RedirectToAction("index");
    }
}
