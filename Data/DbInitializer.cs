using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Ecommerce.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles if they don't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Customer"))
                await roleManager.CreateAsync(new IdentityRole("Customer"));

            // Create default admin user if not exists
            var admin = await userManager.FindByEmailAsync("admin@ecommerce.com");
            if (admin == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@ecommerce.com",
                    Email = "admin@ecommerce.com",
                    FullName = "Admin User",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Admin123!");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
