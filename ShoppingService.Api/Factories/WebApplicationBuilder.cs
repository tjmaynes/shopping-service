using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ShoppingService.Api.Configuration;

namespace ShoppingService.Api.Factories
{
    public static class WebApplicationBuilderFactory
    {
        public static IWebHostBuilder Initialize(string[] args, string currentDirectoryPath, string settingsFileName) =>
            WebHost.CreateDefaultBuilder(args)
                  .ConfigureServices(app => ServiceConfiguration.Configure(app, new ConfigurationBuilder()
                        .SetBasePath(currentDirectoryPath)
                        .AddJsonFile(settingsFileName, optional: false, reloadOnChange: true)
                        .Build()))
                  .Configure(ApplicationConfiguration.Configure);
    }
}
