using System;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Api;

namespace ShoppingService.Api.Tests.Integration
{
    public class CartControllerTests: IClassFixture<ShoppingServiceApiFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CartControllerTests(ShoppingServiceApiFactory<Startup> factory) {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_WhenCalled_ReturnsOkResult()
        {
            var httpResponse = await _client.GetAsync("/api/shopping/cart");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            // var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            // var players = JsonConvert.DeserializeObject<IEnumerable<Player>>(stringResponse);
            // Assert.Contains(players, p => p.FirstName=="Wayne");
            // Assert.Contains(players, p => p.FirstName == "Mario");
        }
    }
}
