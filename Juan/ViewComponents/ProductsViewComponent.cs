using Juan.Models;
using Microsoft.AspNetCore.Mvc;

namespace Juan.ViewComponents;

public class ProductsViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(IEnumerable<Product> products)
    {
        return View(await Task.FromResult(products));
    }
}
