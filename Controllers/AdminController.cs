using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;

namespace Ecommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin Dashboard
        public async Task<IActionResult> Index()
        {
            // Fetch some useful stats
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders
            };

            return View(model);
        }
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var payments = await _context.Payments.ToListAsync();

            // Build AdminOrderViewModel list
            var model = orders.Select(o =>
            {
                var payment = payments.FirstOrDefault(p => p.UserId == o.UserId);
                return new AdminOrdersViewModel
                {
                    Order = o,
                    Payment = payment,
                    UserName = payment?.CardHolder ?? "Unknown", // use CardHolder if we don't have separate name
                    UserEmail = payment?.UserId ?? "Unknown"     // using UserId as email placeholder
                };
            }).ToList();

            return View(model);
        }



        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.UserId == order.UserId && p.Amount == order.TotalAmount);

            // Fetch user info from session (example)
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";
            var userEmail = HttpContext.Session.GetString("UserEmail") ?? "Unknown";

            var model = new AdminOrderDetailsViewModel
            {
                Order = order,
                Payment = payment,
                UserName = userName,
                UserEmail = userEmail
            };

            return View(model);
        }


    }
}
    