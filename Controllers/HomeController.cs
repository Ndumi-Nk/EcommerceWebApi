using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Controllers // ✅ Correct namespace: Controllers, not Controller
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();

            // Load cart from session
            List<CartItemViewModel> cart = HttpContext.Session.GetString("Cart") != null
                ? JsonSerializer.Deserialize<List<CartItemViewModel>>(HttpContext.Session.GetString("Cart"))
                : new List<CartItemViewModel>();

            var model = new HomeViewModel
            {
                IsLoggedIn = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Products = products,
                CartItems = cart
            };

            return View(model);
        }
    }

}
