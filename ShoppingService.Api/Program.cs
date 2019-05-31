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
            var configuration = AppConfigurationBuilder.Initialize(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "settings.json"
            ).Build();

            WebApplicationBuilderFactory
                .Initialize(args, configuration)
                .Build()
                .Run();
        }
    }
}
