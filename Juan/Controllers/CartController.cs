﻿using System.Text.Json;
using Juan.Data;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juan.Controllers;

public class CartController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CartController(JuanDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<CartVM>? cart = JsonSerializer.Deserialize<IEnumerable<CartVM>>(Request.Cookies["cart"]);
        if (cart == null) cart = new List<CartVM>();
        return View(cart);
    }

    public async Task<IActionResult> CartIndexPartial()
    {
        IEnumerable<CartVM>? cart = JsonSerializer.Deserialize<IEnumerable<CartVM>>(Request.Cookies["cart"]);
        if (cart == null) cart = new List<CartVM>();
        return PartialView("_CartIndexPartial", cart);
    }

    public async Task<IActionResult> AddToCart(int? id)
    {
        if (id == null) return BadRequest();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return BadRequest();
        List<CartVM> cart;
        string basket = HttpContext.Request.Cookies["cart"];
        if (string.IsNullOrEmpty(basket)) cart = new();

        else
        {
            cart = JsonSerializer.Deserialize<List<CartVM>>(basket);
            if (cart.Exists(p => p.Id == id))
            {
                cart.Find(p => p.Id == id).Count++;
            }
            else
            {
                cart.Add(new()
                {
                    Id = product.Id,
                    Count = 1,
                    Image = product.MainImage,
                    Name = product.Name,
                    Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price
                });
            }
        }
        if (User.Identity.IsAuthenticated)
        {
            AppUser? user = await _userManager.Users.Include(u => u.CartProducts).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (user == null) return BadRequest();
            if (user.CartProducts.Any(b => b.ProductId == id))
                user.CartProducts.Find(b => b.ProductId == id).Count++;
            else
            {
                CartProduct newBasket = new()
                {
                    ProductId = product.Id,
                    UserId = user.Id,
                    Count = 1
                };
                user.CartProducts.Add(newBasket);
            }
            await _context.SaveChangesAsync();
        }
        HttpContext.Response.Cookies.Append("cart", JsonSerializer.Serialize(cart));

        return PartialView("_CartPartial", cart);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveProduct(int? id)
    {
        if (id == null) return BadRequest();
        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        List<CartVM>? cart = JsonSerializer.Deserialize<List<CartVM>>(Request.Cookies["cart"]);
        if (cart == null) return NotFound();
        CartVM? existProduct = cart.FirstOrDefault(p => p.Id == product.Id);
        if (existProduct == null) return NotFound();
        cart.Remove(existProduct);
        HttpContext.Response.Cookies.Append("cart", JsonSerializer.Serialize(cart));
        if (User.Identity.IsAuthenticated)
        {
            var cartProduct = await _context.CartProducts.Include(p => p.User).FirstOrDefaultAsync(p => p.ProductId == id && p.User.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (cartProduct == null) return BadRequest();
            _context.CartProducts.Remove(cartProduct);
            await _context.SaveChangesAsync();
        }
        return PartialView("_CartPartial", cart);
    }

    public async Task<IActionResult> Decrement(int? id)
    {
        if (id == null) return BadRequest();
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return BadRequest();
        List<CartVM> cart;
        string basket = HttpContext.Request.Cookies["cart"];
        if (string.IsNullOrEmpty(basket)) cart = new();
        else
        {
            cart = JsonSerializer.Deserialize<List<CartVM>>(basket);
            if (cart.Exists(p => p.Id == id))
            {
                var existCart = cart.Find(p => p.Id == id);
                if (existCart.Count > 1) existCart.Count--;
                else cart.Remove(existCart);
            }
        }
        if (User.Identity.IsAuthenticated)
        {
            AppUser? user = await _userManager.Users.Include(u => u.CartProducts).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (user == null) return BadRequest();
            if (user.CartProducts.Any(b => b.ProductId == id))
            {
                var existCart = user.CartProducts.Find(b => b.ProductId == id);
                if (existCart.Count > 1) existCart.Count--;
                else user.CartProducts.Remove(existCart);
                await _context.SaveChangesAsync();
            }
            else
            {
                CartProduct newBasket = new()
                {
                    ProductId = product.Id,
                    UserId = user.Id,
                    Count = 1
                };
                user.CartProducts.Add(newBasket);
            }
            await _context.SaveChangesAsync();
        }
        HttpContext.Response.Cookies.Append("cart", JsonSerializer.Serialize(cart));

        return PartialView("_CartPartial", cart);
    }
}
