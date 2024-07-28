﻿using Juan.Data;
using Juan.Helpers;
using Juan.Models;
using Microsoft.AspNetCore.Mvc;

namespace Juan.Areas.Admin.Controllers;

[Area("admin")]
public class TagController : DefaultCRUD<Tag>
{
    public TagController(JuanDbContext context) : base(context) { }
}
