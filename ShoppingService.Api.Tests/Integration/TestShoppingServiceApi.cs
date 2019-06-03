using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using ShoppingService.Api.Factories;
using Xunit;

namespace ShoppingService.Api.Tests.Integration
{
    public class TestShoppingServiceApi
    {
        private readonly TestServer _server;

        public TestShoppingServiceApi() {
            var environment = Environment.GetEnvironmentVariable("SHOPPING_SERVICE_ENVIRONMENT");
            var configuration = AppConfigurationBuilder.Initialize(
                Directory.GetCurrentDirectory(),
                $"settings.{environment}.json"
            ).Build();

            var dbConnectionString = Environment.GetEnvironmentVariable("SHOPPING_SERVICE_DB_CONNECTION_STRING");
            var webHostBuilder = WebApplicationBuilderFactory.Initialize(configuration, dbConnectionString);

            _server = new TestServer(webHostBuilder);
        }

        [Theory]
        [InlineData("/api/shopping/cart")]
        public async Task Get_WhenCalled_ReturnsOkResult(string url)
        {
            var client = _server.CreateClient();
            var httpResponse = await client.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", httpResponse.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/shopping/cart")]
        public async Task GetById_WhenCalled_ReturnsOkResult(string url)
        {
            var client = _server.CreateClient();
            var httpResponse = await client.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", httpResponse.Content.Headers.ContentType.ToString());
        }
    }
}
