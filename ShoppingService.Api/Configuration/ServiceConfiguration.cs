using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Api.Options;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Repositories;
using ShoppingService.Infrastructure.Data.Clients;

namespace ShoppingService.Api.Configuration {
    public static class ServiceConfiguration {
        public static void Configure(IServiceCollection services, IConfiguration configuration) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connectionStringsOptions =
                configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>();
            var (serviceEndpoint, authKey) = connectionStringsOptions;
            var cosmosDbOptions = configuration.GetSection("DocumentDb").Get<DocumentDbOptions>();
            var (databaseName, collectionNames) = cosmosDbOptions;

           // Cart Endpoint
            services.AddSingleton<IDocumentDbClient<CartItem>>(DocumentDbClientFactory<CartItem>.CreateAndConnect(
                serviceEndpoint, authKey, databaseName, collectionNames["Cart"]
            ));
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IRepository<CartItem>, CartRepository>();
        }
    }
}
