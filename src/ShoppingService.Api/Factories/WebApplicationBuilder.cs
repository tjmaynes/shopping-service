using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingService.Api.Options;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Repositories;
using ShoppingService.Infrastructure.Data.Clients;
using FluentValidation;

namespace ShoppingService.Api.Factories
{
    public static class WebApplicationBuilderFactory
    {
        public static IWebHostBuilder Initialize(IConfiguration configuration, string dbConnectionString) =>
            WebHost.CreateDefaultBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks();

                    var (databaseName, collectionNames) = configuration.GetSection("DocumentDb").Get<DocumentDbOptions>();

                    // Cart Endpoint
                    services.AddSingleton<IDocumentDbClient<CartItem>>(MongoDbCollectionClient<CartItem>.Create(
                        dbConnectionString, databaseName, collectionNames["Cart"]
                    ));
                    services.AddScoped<IRepository<CartItem>, CartRepository>();
                    services.AddScoped<AbstractValidator<CartItem>, CartItemValidator>();
                    services.AddScoped<ICartService, CartService>();

                    services.AddControllers();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => {
                        endpoints.MapHealthChecks("/health");
                        endpoints.MapControllers();
                    });
                });
    }
}
