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
}
