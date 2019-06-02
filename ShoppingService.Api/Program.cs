using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ShoppingService.Api.Factories;

namespace ShoppingService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("SHOPPING_SERVICE_ENVIRONMENT");
            var configuration = AppConfigurationBuilder.Initialize(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                $"settings.{environment}.json"
            ).Build();

            WebApplicationBuilderFactory
                .Initialize(args, configuration)
                .Build()
                .Run();
        }
    }
}
