using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "x-api-key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = config["ApiSettings:ApiKey"];

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var incomingKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "API Key is missing"
                };
                return;
            }

            if (!apiKey.Equals(incomingKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Invalid API Key"
                };
                return;
            }

            await next();
        }
    }
}
