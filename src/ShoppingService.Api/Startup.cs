using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using static LanguageExt.Prelude;

namespace ShoppingService.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            var dbConnectionString = throwIfEnvironmentVariableNotFound("SHOPPING_SERVICE_DB_CONNECTION_STRING");
            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(dbConnectionString));

            services.AddScoped<IDatabaseClient<CartItem>, DatabaseClient<CartItem>>();
            services.AddScoped<IRepository<CartItem>, CartRepository>();
            services.AddScoped<AbstractValidator<CartItem>, CartItemValidator>();
            services.AddScoped<ICartService, CartService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/health"));
            app.UseEndpoints(endpoints => handleCartEndpoints(endpoints));
        }
        private void handleCartEndpoints(IEndpointRouteBuilder endpoints)
        {
            CartService service = endpoints.ServiceProvider.GetService<CartService>();

            endpoints.MapGet("/api/cart", async context =>
            {
                // if (!context.Request.HasJsonContentType())
                // {
                //     context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                //     return;
                // }

                var currentPage = context.Request.RouteValues["currentPage"] as int? ?? 0;
                var pageSize = context.Request.RouteValues["pageSize"] as int? ?? 40;

                await match(service.GetItemsFromCart(currentPage, pageSize),
                    Right: result => context.Response.WriteAsJsonAsync(new { data = result }),
                    Left: error => context.Response.StatusCode = convertErrorCode(error.ErrorCode));
            });
        }

        // [HttpGet("{id}")]
        // public async Task<ActionResult<CartItem>> GetById(string id) =>
        //     await match(_service.GetItemById(id),
        //         Right: item => Ok(new Dictionary<string, CartItem> {{ "data", item }}),
        //         Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
        //     );

        // [HttpPost]
        // public async Task<ActionResult<CartItem>> Post([FromBody] CartItem newItem) =>
        //     await match(_service.AddItemToCart(newItem),
        //         Right: item => StatusCode(201, new Dictionary<string, CartItem> {{ "data", item }}),
        //         Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
        //     );

        // [HttpPut("{id}")]
        // public async Task<ActionResult<CartItem>> Put(string id, [FromBody] CartItem updatedItem) =>
        //     await match(_service.UpdateItemInCart(updatedItem),
        //         Right: item => Ok(new Dictionary<string, CartItem> {{ "data", item }}),
        //         Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
        //     );

        // [HttpDelete("{id}")]
        // public async Task<ActionResult<Guid>> Delete(string id) =>
        //     await match(_service.RemoveItemFromCart(id),
        //         Right: removedId => Ok(new Dictionary<string, string> {{ "data", id }}),
        //         Left: error => StatusCode(ConvertErrorCode(error.ErrorCode), error.Message)
        //     );

        private int convertErrorCode(ServiceErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ServiceErrorCode.InvalidItem: return 422;
                case ServiceErrorCode.ItemNotFound: return 404;
                default: return 500;
            }
        }

        private string throwIfEnvironmentVariableNotFound(string envVarName)
        {
            var value = Environment.GetEnvironmentVariable(envVarName);
            if (value != null && value.Length > 0)
            {
                return value;
            }
            else
            {
                throw new SystemException($"Unable to find environment variable: '{envVarName}'");
            }
        }
    }
}
