using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ShoppingService.Api.Controllers;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using Xunit;
using Moq;

namespace ShoppingService.Api.Tests.Units
{
    public class TestCartController
    {
        [Fact]
        public async Task Get_WhenCalled_ReturnsOkResult_WithCartItems()
        {
            var expected = new List<CartItem>();
            expected.Add(new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemsFromCart())
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var getResults = await sut.Get();
            Assert.IsType<OkObjectResult>(getResults.Result);

            var result = getResults.Result as OkObjectResult;
            var items = Assert.IsType<List<CartItem>>(result.Value);
            Assert.Equal(expected, items);

            serviceMock.Verify(mock => mock.GetItemsFromCart(), Times.Once());
        }

        [Fact]
        public async Task Get_WhenCalled_AndServerError_ReturnsNon200StatusCode()
        {
            var expectedErrorMessages = new List<string> { "Internal Service Error" };
            var expectedErrorStatusCode = 500;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemsFromCart())
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Get();
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(expectedErrorStatusCode, result.StatusCode);

            serviceMock.Verify(mock => mock.GetItemsFromCart(), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnsOkResult_WithCartItem()
        {
            var expectedId = Guid.NewGuid();
            var expected = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemById(It.IsAny<Guid>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.GetById(expectedId);
            Assert.IsType<OkObjectResult>(results.Result);

            var result = results.Result as OkObjectResult;
            var items = Assert.IsType<CartItem>(result.Value);
            Assert.Equal(expected, items);

            serviceMock.Verify(x => x.GetItemById(It.Is<Guid>(id => id == expectedId)), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenCalled_AndServerError_ReturnsNon200StatusCode()
        {
            var expectedId = Guid.NewGuid();
            var expectedErrorMessages = new List<string> { "Internal Service Error" };
            var expectedErrorStatusCode = 500;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemById(It.IsAny<Guid>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.GetById(expectedId);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(expectedErrorStatusCode, result.StatusCode);

            serviceMock.Verify(x => x.GetItemById(It.Is<Guid>(id => id == expectedId)), Times.Once());
        }

        [Fact]
        public async Task Post_WhenCalled_WithValidItem_Returns201StatusCode_WithCartItem()
        {
            var expected = new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.AddItemToCart(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Post(expected);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            var resultItem = Assert.IsType<CartItem>(result.Value);
            Assert.Equal(expected, resultItem);
            Assert.Equal(201, result.StatusCode);

            serviceMock.Verify(x => x.AddItemToCart(It.Is<CartItem>(item => item == expected)), Times.Once());
        }

        [Fact]
        public async Task Post_WhenCalled_WithInvalidItem_ReturnsNon201StatusCode()
        {
            var invalidItem = new CartItem(Guid.NewGuid(), "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expectedErrorMessages = new List<string> { "Invalid item" };
            var expectedErrorStatusCode = 400;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.AddItemToCart(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Post(invalidItem);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(expectedErrorStatusCode, result.StatusCode);

            serviceMock.Verify(x => x.AddItemToCart(It.Is<CartItem>(item => item == invalidItem)), Times.Once());
        }

        [Fact]
        public async Task Put_WhenCalled_WithValidUpdatedItem_Returns200StatusCode()
        {
            var expectedId = Guid.NewGuid();
            var expected = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.UpdateItemInCart(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Put(expectedId, expected);
            Assert.IsType<OkObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            var resultItem = Assert.IsType<CartItem>(result.Value);
            Assert.Equal(expected, resultItem);
            Assert.Equal(200, result.StatusCode);

            serviceMock.Verify(x => x.UpdateItemInCart(It.Is<CartItem>(item => item == expected)), Times.Once());
        }

        [Fact]
        public async Task Put_WhenCalled_WithInvalidItem_ReturnsNon200StatusCode()
        {
            var invalidItemId = Guid.NewGuid();
            var invalidItem = new CartItem(invalidItemId, "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expectedErrorMessages = new List<string> { "Invalid item." };
            var expectedErrorStatusCode = 400;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.UpdateItemInCart(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Put(invalidItemId, invalidItem);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(expectedErrorStatusCode, result.StatusCode);

            serviceMock.Verify(x => x.UpdateItemInCart(It.Is<CartItem>(item => item == invalidItem)), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenCalled_WithValidItem_Returns204StatusCode()
        {
            var expected = Guid.NewGuid();

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.RemoveItemFromCart(It.IsAny<Guid>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Delete(expected);
           Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            var resultItem = Assert.IsType<Guid>(result.Value);
            Assert.Equal(expected, resultItem);
            Assert.Equal(200, result.StatusCode);

            serviceMock.Verify(x => x.RemoveItemFromCart(It.Is<Guid>(item => item == expected)), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenCalled_WhenItemDoesNotExist_ReturnsNon204StatusCode()
        {
            var expectedId = Guid.NewGuid();
            var expectedErrorMessages = new List<string> { "Item does not exist." };
            var expectedErrorStatusCode = 404;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.RemoveItemFromCart(It.IsAny<Guid>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Delete(expectedId);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(expectedErrorStatusCode, result.StatusCode);

            serviceMock.Verify(x => x.RemoveItemFromCart(It.Is<Guid>(item => item == expectedId)), Times.Once());
        }
    }
}
