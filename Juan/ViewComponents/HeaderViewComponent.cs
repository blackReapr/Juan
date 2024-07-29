using Juan.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Juan.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(IEnumerable<CartVM> cartVM)
    {
        return View(await Task.FromResult(cartVM));
    }
}
