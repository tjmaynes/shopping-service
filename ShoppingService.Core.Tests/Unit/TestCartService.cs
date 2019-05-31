using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Core.Tests.Data;
using Xunit;
using Moq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Core.Tests.Unit
{
    public class TestCartService : IClassFixture<CartServiceFixture>
    {
        private readonly CartServiceFixture _fixture;

        public TestCartService(CartServiceFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CartService_WithNoNullArguments_ShouldReturnCartService()
        {
            var repositoryMock = new Mock<IRepository<CartItem>>();
            var sut = _fixture.Initialize(repositoryMock.Object);
            Assert.NotNull(sut);
        }

        [Fact]
        public void CartService_WithNullArgument_ShouldThrowNullException()
        {
            var validator = CartItemValidatorFixture.Initialize();
            var except = Assert.Throws<ArgumentNullException>(() => new CartService(null, validator));
            Assert.Equal("repository", except.ParamName);

            var repository = new CartRepositoryFake();
            except = Assert.Throws<ArgumentNullException>(() => new CartService(repository, null));
            Assert.Equal("validator", except.ParamName);
        }

        [Fact]
        public async Task GetItemsFromCart_WhenCalled_ShouldEventually_ReturnAllItems() {
            var items = new List<CartItem>();
            items.Add(new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            var expected = new PagedResult<CartItem>(items, 1, 1, 1);
            var response = Some(expected);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(response);
            var sut = _fixture.Initialize(repositoryMock.Object);

            var currPage = 0;
            var pageSize = 40;
            await match(sut.GetItemsFromCart(currPage, pageSize),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.GetAll(
                    It.Is<int>(num => num == currPage),
                    It.Is<int>(num => num == pageSize)),
                Times.Once()
            );
        }

        [Fact]
        public async Task GetItemsFromCart_WhenCalled_WithNoItems_ShouldEventually_ReturnServiceError() {
            var expected = Some(PagedResult<CartItem>.CreateEmptyResult());

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            var currPage = 0;
            var pageSize = 40;
            await match(sut.GetItemsFromCart(currPage, pageSize),
                Right: r => Assert.Equal(expected, r),
                Left: _  => Assert.False(true, "Should't get here!")
            );

            repositoryMock.Verify(
                mock => mock.GetAll(
                    It.Is<int>(num => num == currPage),
                    It.Is<int>(num => num == pageSize)),
                Times.Once()
            );
        }

        [Fact]
        public async Task AddItemToCart_WhenCalled_WithValidItem_ShouldEventually_ReturnAllItems() {
            var newItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = Some(newItem);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Add(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.AddItemToCart(newItem),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.Add(It.Is<CartItem>(item => item == expected)),
                Times.Once()
            );
        }

        [Fact]
        public async Task AddItemToCart_WhenCalled_WithInvalidItem_ShouldEventually_ReturnServiceError() {
            var invalidItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expected = new ServiceError("'Price' must be greater than '0'.", ServiceErrorCode.InvalidItem);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.AddItemToCart(invalidItem),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: l => Assert.Equal(l, expected)
            );

            repositoryMock.Verify(mock => mock.Add(null), Times.Never());
        }
    
        [Fact]
        public async Task GetItemById_WhenCalled_WithValidItem_ShouldEventually_ReturnASpecificItem() {
            var expectedId = Guid.NewGuid().ToString();
            var item = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = Some(item);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetById(It.IsAny<string>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.GetItemById(expectedId),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.GetById(It.Is<string>(id => id == expectedId)),
                Times.Once()
            );
        }

        [Fact]
        public async Task GetItemById_WhenCalled_AndNoItemFound_ShouldEventually_ReturnServiceError() {
            var expectedId = Guid.NewGuid().ToString();
            var expected = new ServiceError("Item not found!", ServiceErrorCode.ItemNotFound);
            var response = RightAsync<Exception, Option<CartItem>>(None);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetById(It.IsAny<string>()))
                .Returns(response);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.GetItemById(expectedId),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: ex => Assert.Equal(expected, ex) 
            );

            repositoryMock.Verify(
                mock => mock.GetById(It.Is<string>(id => id == expectedId)),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdateItemInCart_WhenCalled_WithValidItem_ShouldEventually_ReturnUpdatedItem() {
            var updatedItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = Some(updatedItem);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Update(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.UpdateItemInCart(updatedItem),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.Update(It.Is<CartItem>(item => item == updatedItem)),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdateItemInCart_WhenCalled_WithInvalidItem_ShouldEventually_ReturnServiceError() {
            var invalidItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expected = new ServiceError("'Price' must be greater than '0'.", ServiceErrorCode.InvalidItem);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.UpdateItemInCart(invalidItem),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: ex => Assert.Equal(expected, ex)
            );

            repositoryMock.Verify(mock => mock.Update(null), Times.Never());
        }

        [Fact]
        public async Task RemoveItemInCart_WhenCalled_WithValidItem_ShouldEventually_ReturnUpdatedItem() {
            var expected = Guid.NewGuid().ToString();
            var item = new CartItem(expected, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = Some(item);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Remove(It.IsAny<string>()))
                .Returns(response);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.RemoveItemFromCart(expected),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.Remove(It.Is<string>(id => id == expected)),
                Times.Once()
            );
        }

        [Fact]
        public async Task RemoveItemFromCart_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnServiceError() {
            var exceptionMessage = "Unknown error has occurred";
            var exception = new Exception(exceptionMessage);
            var expected = new ServiceError(exceptionMessage, ServiceErrorCode.UnknownException);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Remove(It.IsAny<string>()))
                .Returns(exception);
            var sut = _fixture.Initialize(repositoryMock.Object);

            var removedId = Guid.NewGuid().ToString();
            await match(sut.RemoveItemFromCart(removedId),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: ex => Assert.Equal(expected, ex)
            );

            repositoryMock.Verify(
                mock => mock.Remove(It.Is<string>(id => id == removedId)),
                Times.Once()
            );
        }
    }
}
