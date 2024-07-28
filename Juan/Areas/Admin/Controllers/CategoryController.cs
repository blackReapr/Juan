using Juan.Data;
using Juan.Helpers;
using Juan.Models;

namespace Juan.Areas.Admin.Controllers;

public class CategoryController : DefaultCRUD<Category>
{
    public CategoryController(JuanDbContext context) : base(context) { }
}
