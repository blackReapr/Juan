using Juan.Data;
using Juan.Interfaces;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Juan.Services;

public class LayoutService : ILayoutService
{
    private readonly JuanDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LayoutService(JuanDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<CartVM>> GetCartAsync()
    {
        if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            var cartCookie = _httpContextAccessor.HttpContext.Request.Cookies["cart"];
            if (string.IsNullOrEmpty(cartCookie)) return new List<CartVM>();
            IEnumerable<CartVM> cart = JsonSerializer.Deserialize<IEnumerable<CartVM>>(cartCookie);
            foreach (CartVM cartVM in cart)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartVM.Id);
                cartVM.Name = product.Name;
                cartVM.Image = product.MainImage;
                cartVM.Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price;
            }
            return cart;
        }
        else
        {
            var cartProducts = _context.CartProducts.Include(c => c.User).Include(c => c.Product).Where(c => c.User.NormalizedUserName == _httpContextAccessor.HttpContext.User.Identity.Name.ToUpperInvariant());
            List<CartVM> cart = new List<CartVM>();
            foreach (var cartProduct in cartProducts)
            {
                var cartVM = new CartVM()
                {
                    Id = cartProduct.ProductId,
                    Name = cartProduct.Product.Name,
                    Count = cartProduct.Count,
                    Image = cartProduct.Product.MainImage,
                    Price = cartProduct.Product.DiscountPrice > 0 ? cartProduct.Product.DiscountPrice : cartProduct.Product.Price
                };
                cart.Add(cartVM);
            }
            _httpContextAccessor.HttpContext.Response.Cookies.Append("cart", JsonSerializer.Serialize(cart));
            return cart;
        }

    }


    public async Task UpdateWishlistAsync()
    {
        if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            var wishlistCookie = _httpContextAccessor.HttpContext.Request.Cookies["wishlist"];
            if (string.IsNullOrEmpty(wishlistCookie)) return;
            IEnumerable<WishlistVM> wishlist = JsonSerializer.Deserialize<IEnumerable<WishlistVM>>(wishlistCookie);
            foreach (WishlistVM wishlistVM in wishlist)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wishlistVM.Id);
                wishlistVM.Name = product.Name;
                wishlistVM.Image = product.MainImage;
                wishlistVM.IsInStock = product.Count > 0;
                wishlistVM.Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price;
            }
            return;
        }
        else
        {
            var wishlistProducts = _context.Wishlists.Include(c => c.Product).Where(c => c.User.NormalizedUserName == _httpContextAccessor.HttpContext.User.Identity.Name.ToUpperInvariant());
            List<WishlistVM> wishlist = new List<WishlistVM>();
            foreach (var wishlistProduct in wishlistProducts)
            {
                var wishlistVM = new WishlistVM()
                {
                    Id = wishlistProduct.ProductId,
                    Name = wishlistProduct.Product.Name,
                    IsInStock = wishlistProduct.Product.Count > 0,
                    Image = wishlistProduct.Product.MainImage,
                    Price = wishlistProduct.Product.DiscountPrice > 0 ? wishlistProduct.Product.DiscountPrice : wishlistProduct.Product.Price
                };
                wishlist.Add(wishlistVM);
            }
            _httpContextAccessor.HttpContext.Response.Cookies.Append("wishlist", JsonSerializer.Serialize(wishlist));
            return;
        }
    }

}
