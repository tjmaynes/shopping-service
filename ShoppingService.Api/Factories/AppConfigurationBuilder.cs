using Microsoft.Extensions.Configuration;

namespace ShoppingService.Api.Factories
{
    public static class AppConfigurationBuilder
    {
        public static IConfigurationBuilder Initialize(string directoryBasePath, string jsonFilename) =>
            new ConfigurationBuilder()
                .SetBasePath(directoryBasePath)
                .AddJsonFile(jsonFilename, optional: false, reloadOnChange: true);
    }
}
