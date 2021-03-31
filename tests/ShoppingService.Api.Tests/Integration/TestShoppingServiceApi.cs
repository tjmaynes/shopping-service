using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ShoppingService.Api.Tests.Integration
{
    public class TestShoppingServiceApi : IClassFixture<WebApplicationFactory<ShoppingService.Api.Startup>>
    {
        private readonly WebApplicationFactory<ShoppingService.Api.Startup> _factory;

        public TestShoppingServiceApi(WebApplicationFactory<ShoppingService.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnsOkResult()
        {
            var client = _factory.CreateClient();
            var httpResponse = await client.GetAsync("/api/cart");
            httpResponse.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", httpResponse.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnsOkResult()
        {
            var client = _factory.CreateClient();
            var httpResponse = await client.GetAsync("/api/cart/1");
            httpResponse.EnsureSuccessStatusCode();

            Assert.Equal("application/json; charset=utf-8", httpResponse.Content.Headers.ContentType.ToString());
        }
    }
}
