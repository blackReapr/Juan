using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;

namespace Juan.Areas.Admin.Controllers;

[Area("admin")]
public class ColorController : DefaultCRUD<Color>
{
    public ColorController(JuanDbContext context) : base(context) { }
}
