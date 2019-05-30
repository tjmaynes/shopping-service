using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShoppingService.Api.Options;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Repositories;
using ShoppingService.Infrastructure.Data.Clients;

namespace ShoppingService.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connectionStringsOptions =
                Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>();
            var (serviceEndpoint, authKey) = connectionStringsOptions;
            var cosmosDbOptions = Configuration.GetSection("DocumentDb").Get<DocumentDbOptions>();
            var (databaseName, collectionNames) = cosmosDbOptions;

           // Cart Endpoint
            services.AddSingleton<IDocumentDbClient<CartItem>>(DocumentDbClientFactory<CartItem>.CreateAndConnect(
                serviceEndpoint, authKey, databaseName, collectionNames["Cart"]
            ));
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IRepository<CartItem>, CartRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
        }
    }
}
