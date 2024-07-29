using Juan.Data;
using Juan.Enums;
using Juan.Hubs;
using Juan.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Juan.Areas.Admin.Controllers;

[Area("admin"), Authorize(Roles = "admin")]
public class OrderController : Controller
{
    private readonly JuanDbContext _context;
    private readonly IHubContext<StatusHub> _hubContext;

    public OrderController(JuanDbContext context, IHubContext<StatusHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.User).AsNoTracking().ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> UpdateStatus(int? id)
    {
        if (id == null) return BadRequest();
        Order? order = await _context.Orders.Include(o => o.User).SingleOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> UpdateStatus(int? id, StatusEnum status)
    {
        if (id == null) return BadRequest();
        Order? order = await _context.Orders.Include(o => o.User).SingleOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        order.Status = status;
        await _context.SaveChangesAsync();
        string? clientId = order.User.ClientId;
        if (clientId != null) await _hubContext.Clients.Client(clientId).SendAsync("statusUpdate", $"Order {order.Id} status updated to: {status}");
        return RedirectToAction("index");
    }
}
