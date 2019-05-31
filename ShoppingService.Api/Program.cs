using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ShoppingService.Api.Factories;

namespace ShoppingService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilderFactory
                .Initialize(
                    args,
                    Directory.GetCurrentDirectory(),
                    "appsettings.json"
                )
                .Build()
                .Run();
        }
    }
}
