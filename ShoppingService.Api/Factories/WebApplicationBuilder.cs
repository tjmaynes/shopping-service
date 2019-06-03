using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
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
                    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

                    var documentDbOptions = configuration.GetSection("DocumentDb").Get<DocumentDbOptions>();
                    var (databaseName, collectionNames) = documentDbOptions;

                    // Cart Endpoint
                    services.AddSingleton<IDocumentDbClient<CartItem>>(MongoDbCollectionClient<CartItem>.Create(
                        dbConnectionString, databaseName, collectionNames["Cart"]
                    ));
                    services.AddScoped<IRepository<CartItem>, CartRepository>();
                    services.AddScoped<AbstractValidator<CartItem>, CartItemValidator>();
                    services.AddScoped<ICartService, CartService>();
                })
                .Configure(app =>
                {
                    app.UseMvc();
                });
    }
}
