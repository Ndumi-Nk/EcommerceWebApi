using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // Add to Cart
        // -----------------------------
        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (existingItem != null)
            {
                if (existingItem.Quantity < product.StockQuantity)
                    existingItem.Quantity++;
                else
                    TempData["Error"] = $"Cannot add more than {product.StockQuantity} of {product.Name}.";
            }
            else
            {
                if (product.StockQuantity > 0)
                {
                    cart.Add(new CartItemViewModel
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = 1,
                        Stock = product.StockQuantity
                    });
                }
                else
                {
                    TempData["Error"] = $"{product.Name} is out of stock.";
                }
            }

            SaveCartToSession(cart);
            TempData["Success"] = $"{product.Name} added to cart!";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // -----------------------------
        // View Cart
        // -----------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        // -----------------------------
        // Increase Quantity
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increase(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null && item.Quantity < product.StockQuantity)
                    item.Quantity++;
                else
                    TempData["Error"] = $"Cannot add more than {product.StockQuantity} of {item.ProductName}.";
            }

            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        // -----------------------------
        // Decrease Quantity
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrease(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);
            }

            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        // -----------------------------
        // Remove Item
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == id);
            if (item != null)
                cart.Remove(item);

            SaveCartToSession(cart);
            return RedirectToAction("Index");
        }

        // -----------------------------
        // Clear Cart
        // -----------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        // -----------------------------
        // Helper Methods
        // -----------------------------
        private List<CartItemViewModel> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            return cartJson != null
                ? JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson)
                : new List<CartItemViewModel>();
        }

        private void SaveCartToSession(List<CartItemViewModel> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }
}
