using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShoppingService.Api;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Repositories;
using ShoppingService.Infrastructure.Data.Clients;

namespace ShoppingService.Api.Tests.Integration
{
    public class ShoppingServiceApiFactory<TStartup> : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // builder.ConfigureServices(services =>
            // {
            //     var serviceProvider = new ServiceCollection()
            //         .BuildServiceProvider();

            //     services.AddSingleton<IDocumentDbClient>(x => DocumentDbClientFactory.CreateAndConnect(

            //     ));

            //     services.AddTransient<IRepository<CartItem>>(x => new CartRepository(
            //         x.GetService<IDocumentDbClient>()
            //     ));

            //     var sp = services.BuildServiceProvider();
            //     using (var scope = sp.CreateScope())
            //     {
            //         var scopedServices = scope.ServiceProvider;
            //         var cartRepository = scopedServices.GetRequiredService<CartRepository>();
            //         var logger = scopedServices.GetRequiredService<ILogger<ShoppingServiceApiFactory<TStartup>>>();

            //         // cartRepository.EnsureExists();
            //         cartRepository.Add(new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            //         cartRepository.Add(new CartItem(Guid.NewGuid(), "some-name-2", 2.99m, "some-manufacturer", DateTime.UtcNow));
            //     }
            // });
        }
    }
}
