// Middleware/ApiKeyMiddleware.cs
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Ecommerce.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY = "wK3f9JH2k7pG+9l3aH8sR0v7X2dQ9WmM1gR5tYxKZ0U="; // Replace with your key

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("x-api-key", out var key) || key != API_KEY)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: API Key missing or invalid.");
                return;
            }
            await _next(context);
        }
    }


}
