using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Juan.Areas.Admin.Controllers;

[Area("admin"), Authorize(Roles = "admin")]
public class SizeController : DefaultCRUD<Size>
{
    public SizeController(JuanDbContext context) : base(context) { }
}
