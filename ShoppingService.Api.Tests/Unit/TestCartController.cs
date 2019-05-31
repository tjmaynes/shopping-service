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
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Api.Tests.Units
{
    public class TestCartController
    {
        [Fact]
        public async Task Get_WhenCalled_ReturnsOkResult_WithCartItems()
        {
            var items = new List<CartItem>();
            items.Add(new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            var pageResult = new PagedResult<CartItem>(items, 1, 1, 1);
            var expected = new Dictionary<string, PagedResult<CartItem>> {{ "data", pageResult }};
            var response = RightAsync<ServiceError, PagedResult<CartItem>>(pageResult);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemsFromCart(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(response);
            var sut = new CartController(serviceMock.Object);

            var getResults = await sut.Get();
            Assert.IsType<OkObjectResult>(getResults.Result);

            var result = getResults.Result as OkObjectResult;
            var data = Assert.IsType<Dictionary<string, PagedResult<CartItem>>>(result.Value);
            Assert.Equal(expected, data);

            serviceMock.Verify(mock => mock.GetItemsFromCart(
                It.Is<int>(num => num == 0),
                It.Is<int>(num => num == 40)
            ), Times.Once());
        }

        [Fact]
        public async Task Get_WhenCalled_AndServerError_ReturnsNon200StatusCode()
        {
            var expectedErrorMessages = "Internal Service Error";
            var expectedErrorStatusCode = ServiceErrorCode.UnknownException;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemsFromCart(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Get();
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(500, result.StatusCode);

            serviceMock.Verify(mock => mock.GetItemsFromCart(
                It.Is<int>(num => num == 0),
                It.Is<int>(num => num == 40)
            ), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenCalled_ReturnsOkResult_WithCartItem()
        {
            var expectedId = Guid.NewGuid().ToString();
            var item = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = new Dictionary<string, CartItem> {{ "data", item }};
            var response = RightAsync<ServiceError, CartItem>(item);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemById(It.IsAny<string>()))
                .Returns(response);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.GetById(expectedId);
            Assert.IsType<OkObjectResult>(results.Result);

            var result = results.Result as OkObjectResult;
            var items = Assert.IsType<Dictionary<string, CartItem>>(result.Value);
            Assert.Equal(expected, items);

            serviceMock.Verify(x => x.GetItemById(It.Is<string>(id => id == expectedId)), Times.Once());
        }

        [Fact]
        public async Task GetById_WhenCalled_AndServerError_ReturnsNon200StatusCode()
        {
            var expectedId = Guid.NewGuid().ToString();
            var expectedErrorMessage = "Internal Service Error";
            var expectedErrorStatusCode = ServiceErrorCode.UnknownException;
            var expected = new ServiceError(expectedErrorMessage, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.GetItemById(It.IsAny<string>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.GetById(expectedId);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessage, result.Value);
            Assert.Equal(500, result.StatusCode);

            serviceMock.Verify(x => x.GetItemById(It.Is<string>(id => id == expectedId)), Times.Once());
        }

        [Fact]
        public async Task Post_WhenCalled_WithValidItem_Returns201StatusCode_WithCartItem()
        {
            var newItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = new Dictionary<string, CartItem> {{ "data", newItem }};
            var response = RightAsync<ServiceError, CartItem>(newItem);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.AddItemToCart(It.IsAny<CartItem>()))
                .Returns(response);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Post(newItem);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            var resultItem = Assert.IsType<Dictionary<string, CartItem>>(result.Value);
            Assert.Equal(expected, resultItem);
            Assert.Equal(201, result.StatusCode);

            serviceMock.Verify(x => x.AddItemToCart(It.Is<CartItem>(item => item == newItem)), Times.Once());
        }

        [Fact]
        public async Task Post_WhenCalled_WithInvalidItem_ReturnsNon201StatusCode()
        {
            var invalidItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expectedErrorMessages = "Invalid item" ;
            var expectedErrorStatusCode = ServiceErrorCode.InvalidItem;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.AddItemToCart(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Post(invalidItem);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(422, result.StatusCode);

            serviceMock.Verify(x => x.AddItemToCart(It.Is<CartItem>(item => item == invalidItem)), Times.Once());
        }

        [Fact]
        public async Task Put_WhenCalled_WithValidItem_Returns200StatusCode()
        {
            var expectedId = Guid.NewGuid().ToString();
            var updatedItem = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = new Dictionary<string, CartItem> {{ "data", updatedItem }};
            var response = RightAsync<ServiceError, CartItem>(updatedItem);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.UpdateItemInCart(It.IsAny<CartItem>()))
                .Returns(response);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Put(expectedId, updatedItem);
            Assert.IsType<OkObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            var resultItem = Assert.IsType<Dictionary<string, CartItem>>(result.Value);
            Assert.Equal(expected, resultItem);
            Assert.Equal(200, result.StatusCode);

            serviceMock.Verify(x => x.UpdateItemInCart(It.Is<CartItem>(item => item == updatedItem)), Times.Once());
        }

        [Fact]
        public async Task Put_WhenCalled_WithInvalidItem_ReturnsNon200StatusCode()
        {
            var invalidItemId = Guid.NewGuid().ToString();
            var invalidItem = new CartItem(invalidItemId, "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expectedErrorMessages = "Invalid item.";
            var expectedErrorStatusCode = ServiceErrorCode.InvalidItem;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.UpdateItemInCart(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Put(invalidItemId, invalidItem);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(422, result.StatusCode);

            serviceMock.Verify(x => x.UpdateItemInCart(It.Is<CartItem>(item => item == invalidItem)), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenCalled_WithValidItem_Returns204StatusCode()
        {
            var expectedId = Guid.NewGuid().ToString();
            var expected = new Dictionary<string, string> {{ "data", expectedId }};
            var response = RightAsync<ServiceError, string>(expectedId);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.RemoveItemFromCart(It.IsAny<string>()))
                .Returns(response);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Delete(expectedId);
           Assert.IsType<OkObjectResult>(results.Result);

            var result = results.Result as OkObjectResult;
            var resultItem = Assert.IsType<Dictionary<string, string>>(result.Value);
            Assert.Equal(expected, resultItem);
            Assert.Equal(200, result.StatusCode);

            serviceMock.Verify(x => x.RemoveItemFromCart(It.Is<string>(item => item == expectedId)), Times.Once());
        }

        [Fact]
        public async Task Delete_WhenCalled_WhenItemDoesNotExist_ReturnsNon204StatusCode()
        {
            var expectedId = Guid.NewGuid().ToString();
            var expectedErrorMessages = "Item does not exist.";
            var expectedErrorStatusCode = ServiceErrorCode.ItemNotFound;
            var expected = new ServiceError(expectedErrorMessages, expectedErrorStatusCode);

            var serviceMock = new Mock<ICartService>();
            serviceMock.Setup(mock => mock.RemoveItemFromCart(It.IsAny<string>()))
                .Returns(expected);
            var sut = new CartController(serviceMock.Object);

            var results = await sut.Delete(expectedId);
            Assert.IsType<ObjectResult>(results.Result);

            var result = results.Result as ObjectResult;
            Assert.Equal(expectedErrorMessages, result.Value);
            Assert.Equal(404, result.StatusCode);

            serviceMock.Verify(x => x.RemoveItemFromCart(It.Is<string>(item => item == expectedId)), Times.Once());
        }
    }
}
