using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var model = new CheckoutViewModel
            {
                CartItems = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var cart = GetCartFromSession();
            if (!cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            // Decrease stock
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    continue;

                if (product.StockQuantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {product.Name}");
                    model.CartItems = cart;
                    return View(model);
                }

                product.StockQuantity -= item.Quantity;
                _context.Products.Update(product);
            }

            // Save order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.Sum(x => x.Price * x.Quantity),
                Status = "Paid"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Save order items
            foreach (var item in cart)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderItems.Add(orderItem);
            }

            // Save payment
            var payment = new Payment
            {
                UserId = userId,
                CardHolder = model.CardHolder,
                CardNumber = model.CardNumber,
                CVV = model.CVV,
                ExpiryDate = model.ExpiryDate,
                Amount = order.TotalAmount
            };
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync();

            // Clear cart
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Payment successful! Your order has been placed.";
            return RedirectToAction("Receipt", new { orderId = order.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Receipt(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound();

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.UserId == order.UserId && p.Amount == order.TotalAmount);

            var model = new ReceiptViewModel
            {
                Order = order,
                Payment = payment
            };

            return View(model);
        }


        // Helper
        private List<CartItemViewModel> GetCartFromSession() =>
            HttpContext.Session.GetString("Cart") != null
                ? JsonSerializer.Deserialize<List<CartItemViewModel>>(HttpContext.Session.GetString("Cart"))
                : new List<CartItemViewModel>();
    }
}
