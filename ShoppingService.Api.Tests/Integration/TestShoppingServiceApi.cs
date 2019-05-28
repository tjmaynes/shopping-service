using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Api;
using Xunit;

namespace ShoppingService.Api.Tests.Integration
{
    public class TestShoppingServiceApi: IClassFixture<ShoppingServiceApiFactory<Startup>>
    {
        private readonly HttpClient _client;

        public TestShoppingServiceApi(ShoppingServiceApiFactory<Startup> factory) {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("/api/shopping/cart")]
        public async Task Get_WhenCalled_ReturnsOkResult(string url)
        {
            // var httpResponse = await _client.GetAsync(url);
            // httpResponse.EnsureSuccessStatusCode();

            // Assert.Equal("application/json; charset=utf-8", httpResponse.Content.Headers.ContentType.ToString());
        }
    }
}
