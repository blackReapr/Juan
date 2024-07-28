﻿using Juan.Data;
using Juan.Extensions;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.Admin.Controllers;

[Area("admin")]
public class BlogController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public BlogController(JuanDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Blog> blogs = await _context.Blogs.Include(b => b.User).ToListAsync();
        return View(blogs);
    }

    public IActionResult Create() => View();

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Create(Blog blog)
    {
        if (!ModelState.IsValid) return View(blog);
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return BadRequest();
        IEnumerable<string> errors = blog.Photo.VerifyFile();
        if (errors.Any())
        {
            foreach (string error in errors) ModelState.AddModelError("Photo", error);
            return View(blog);
        }
        string filename = await blog.Photo.SaveFileAsync("blog");
        blog.Image = filename;
        blog.UserId = appUser.Id;
        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null) return BadRequest();
        Blog? blog = await _context.Blogs.Include(b => b.User).FirstOrDefaultAsync(s => s.Id == id);
        if (blog == null) return BadRequest();
        if (blog.User.UserName != User.Identity.Name) return BadRequest();
        return View(blog);
    }


    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Update(int? id, Blog blog)
    {
        if (id == null) return BadRequest();
        Blog? existingBlog = await _context.Blogs.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
        if (existingBlog == null) return NotFound();
        if (existingBlog.User.UserName != User.Identity.Name) return BadRequest();

        if (!ModelState.IsValid) return View(blog);

        if (blog.Photo != null)
        {
            IEnumerable<string> errors = blog.Photo.VerifyFile();
            if (errors.Any())
            {
                foreach (string error in errors) ModelState.AddModelError("Photo", error);
                return View(blog);
            }

            DeleteFile.Delete(existingBlog.Image, "slider");

            string filename = await blog.Photo.SaveFileAsync("slider");
            existingBlog.Image = filename;
        }

        existingBlog.Title = blog.Title;
        existingBlog.Description = blog.Description;

        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }


    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return BadRequest();
        Blog? blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
        if (blog == null) return NotFound();
        if (blog.User.UserName != User.Identity.Name) return BadRequest();
        DeleteFile.Delete(blog.Image, "blog");
        _context.Blogs.Remove(blog);
        await _context.SaveChangesAsync();
        return RedirectToAction("index");
    }
}
