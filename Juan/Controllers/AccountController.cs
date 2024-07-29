using iTextSharp.text;
using iTextSharp.text.pdf;
using Juan.Data;
using Juan.Extensions;
using Juan.Helpers;
using Juan.Interfaces;
using Juan.Models;
using Juan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Juan.Controllers;

[Authorize(Roles = "member")]
public class AccountController : Controller
{
    private readonly JuanDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;

    public AccountController(JuanDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return BadRequest();
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == appUser.Id).AsNoTracking().ToListAsync();
        ViewBag.Orders = orders;
        AccountVM accountVM = new()
        {
            Email = appUser.Email,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            UserName = appUser.UserName

        };
        return View(accountVM);
    }

    [HttpPost, AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Index(AccountVM accountVM)
    {
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null) return BadRequest();
        IEnumerable<Order> orders = await _context.Orders.Include(o => o.OrderItems).Where(o => o.UserId == appUser.Id).AsNoTracking().ToListAsync();
        ViewBag.Orders = orders;
        if (!ModelState.IsValid) return View(accountVM);

        IEnumerable<string> errors = accountVM.Profile.VerifyFile();
        if (errors.Any())
        {
            foreach (string error in errors) ModelState.AddModelError("Profile", error);
            return View(accountVM);
        }

        string filename = await accountVM.Profile.SaveFileAsync();
        string? oldFile = appUser.Profile == "default.jpg" ? null : appUser.Profile;

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(appUser, accountVM.CurrentPassword);
        if (!isPasswordCorrect)
        {
            ModelState.AddModelError("Error", "Incorrect Password");
            return View(accountVM);
        }

        if (accountVM.Password != null)
        {
            IdentityResult result = await _userManager.ChangePasswordAsync(appUser, accountVM.CurrentPassword, accountVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("Error", error.Description);
                return View(accountVM);
            }
        }

        appUser.FirstName = accountVM.FirstName;
        appUser.LastName = accountVM.LastName;
        appUser.Email = accountVM.Email;
        appUser.UserName = accountVM.UserName;
        appUser.Profile = filename;

        IdentityResult updateResult = await _userManager.UpdateAsync(appUser);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors) ModelState.AddModelError("Error", error.Description);
            return View(accountVM);
        }

        if (oldFile != null) DeleteFile.Delete(filename, "users");

        await _signInManager.SignInAsync(appUser, true);
        return View();
    }

    public async Task<IActionResult> SendEmail(int? id)
    {
        if (id == null) return BadRequest();
        Order? order = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        AppUser? appUser = await _userManager.FindByNameAsync(User.Identity.Name);
        if (appUser == null || appUser.Id != order.UserId) return BadRequest();
        string body = "";
        using (StreamReader stream = new("wwwroot/templates/orderCheck.html"))
        {
            body = stream.ReadToEnd();
        };

        StringBuilder mainContent = new();
        foreach (OrderItem orderItem in order.OrderItems)
        {
            string content = $"<tr>\r\n<td>{orderItem.Product.Name}</td>\r\n<td class=\"text-right\">${(orderItem.Price * orderItem.Count).ToString("0.00")}</td>\r\n</tr>";
            mainContent.Append(content);
        }
        decimal amountPaid = order.OrderItems.Sum(oi => order.DiscountRate == 0 ? oi.Price * oi.Count : oi.Price * oi.Count * (100 - order.DiscountRate) / 100);
        string total = $"<tr>\r\n<td class=\"fw-700 border-top\">Amount paid</td>\r\n<td class=\"fw-700 text-right border-top\">${amountPaid.ToString("0.00")}</td>\r\n</tr>";
        mainContent.Append(total);


        body = body.Replace("{{id}}", order.Id.ToString());
        body = body.Replace("{{username}}", appUser.UserName);
        body = body.Replace("{{content}}", mainContent.ToString());
        body = body.Replace("{{order_time}}", order.CreatedAt.AddDays(7).ToShortDateString());

        _emailService.SendEmail(appUser.Email, "Order Receipt", body);

        return RedirectToAction("index");
    }

    public IActionResult DownloadOrder(int id)
    {
        var order = _context.Orders.Include(o => o.OrderItems).AsNoTracking().FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound();



        MemoryStream memoryStream = new MemoryStream();
        GeneratePdf(memoryStream, order);

        return File(memoryStream.ToArray(), "application/pdf", $"Order_{order.Id}.pdf");
    }
    private void GeneratePdf(Stream stream, Order order)
    {
        using (var doc = new Document())
        {
            PdfWriter.GetInstance(doc, stream);
            doc.Open();

            // Centered title
            var title = new Paragraph($"Order ID: {order.Id}", new Font(Font.FontFamily.HELVETICA, 18f, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Centered date
            var date = new Paragraph($"Date: {order.CreatedAt.ToShortDateString()}", new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL));
            date.Alignment = Element.ALIGN_CENTER;
            doc.Add(date);

            // Centered status
            var status = new Paragraph($"Status: {order.Status}", new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL));
            status.Alignment = Element.ALIGN_CENTER;
            doc.Add(status);

            // Centered total
            var total = new Paragraph($"Total: ${order.OrderItems.Sum(oi => order.DiscountRate == 0 ? oi.Price * oi.Count : oi.Price * oi.Count * (100 - order.DiscountRate) / 100).ToString("0.00")}", new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL));
            total.Alignment = Element.ALIGN_CENTER;
            doc.Add(total);

            // Centered order items title
            var orderItemsTitle = new Paragraph("Order Items:", new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD));
            orderItemsTitle.Alignment = Element.ALIGN_CENTER;
            doc.Add(orderItemsTitle);

            // Centered order items table
            var table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            var cell = new PdfPCell(new Phrase("Product ID"))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Count"))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Price"))
            {
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);

            // Adding data
            foreach (var item in order.OrderItems)
            {
                cell = new PdfPCell(new Phrase(item.ProductId.ToString()))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(item.Count.ToString()))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase($"${item.Price.ToString("0.00")}"))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                table.AddCell(cell);
            }

            doc.Add(table);

            doc.Close();
        }

    }
}
