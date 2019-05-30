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
            var expected = new List<CartItem>();
            expected.Add(new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetAll(It.IsAny<int>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.GetItemsFromCart(),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.GetAll(It.Is<int>(num => num == 200)),
                Times.Once()
            );
        }

        [Fact]
        public async Task GetItemsFromCart_WhenCalled_WithNoItems_ShouldEventually_ReturnServiceError() {
            var response = new ArgumentNullException();

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetAll(It.IsAny<int>()))
                .Returns(response);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.GetItemsFromCart(),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: l => Assert.Equal(l, ServiceError.CreateWithSingleMessage("Value cannot be null.", 500))
            );

            repositoryMock.Verify(
                mock => mock.GetAll(It.Is<int>(num => num == 200)),
                Times.Once()
            );
        }

        [Fact]
        public async Task AddItemToCart_WhenCalled_WithValidItem_ShouldEventually_ReturnAllItems() {
            var expected = new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Add(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.AddItemToCart(expected),
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
            var invalidItem = new CartItem(Guid.NewGuid(), "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expected = ServiceError.CreateWithSingleMessage("'Price' must be greater than '0'.", 422);

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
            var expectedId = Guid.NewGuid();
            var expected = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.GetItemById(expectedId),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.GetById(It.Is<Guid>(id => id == expectedId)),
                Times.Once()
            );
        }

        [Fact]
        public async Task GetItemById_WhenCalled_WithNotItem_ShouldEventually_ReturnServiceError() {
            var expectedId = Guid.NewGuid();
            var response = new ArgumentNullException("Unable to find item.");

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns(response);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.GetItemById(expectedId),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: l => Assert.Equal(l, ServiceError.CreateWithSingleMessage(response.Message, 500))
            );

            repositoryMock.Verify(
                mock => mock.GetById(It.Is<Guid>(id => id == expectedId)),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdateItemInCart_WhenCalled_WithValidItem_ShouldEventually_ReturnUpdatedItem() {
            var expected = new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Update(It.IsAny<CartItem>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.UpdateItemInCart(expected),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.Update(It.Is<CartItem>(item => item == expected)),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdateItemInCart_WhenCalled_WithInvalidItem_ShouldEventually_ReturnServiceError() {
            var invalidItem = new CartItem(Guid.NewGuid(), "some-name-1", -1m, "some-manufacturer", DateTime.UtcNow);
            var expected = ServiceError.CreateWithSingleMessage("'Price' must be greater than '0'.", 422);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.UpdateItemInCart(invalidItem),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: l => Assert.Equal(l, expected)
            );

            repositoryMock.Verify(mock => mock.Update(null), Times.Never());
        }

        [Fact]
        public async Task RemoveItemInCart_WhenCalled_WithValidItem_ShouldEventually_ReturnUpdatedItem() {
            var expected = Guid.NewGuid();
            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Remove(It.IsAny<Guid>()))
                .Returns(expected);
            var sut = _fixture.Initialize(repositoryMock.Object);

            await match(sut.RemoveItemFromCart(expected),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            repositoryMock.Verify(
                mock => mock.Remove(It.Is<Guid>(id => id == expected)),
                Times.Once()
            );
        }

        [Fact]
        public async Task RemoveItemFromCart_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnServiceError() {
            var exceptionMessage = "Unknown error has occurred";
            var exception = new Exception(exceptionMessage);
            var expected = ServiceError.CreateWithSingleMessage(exceptionMessage, 500);

            var repositoryMock = new Mock<IRepository<CartItem>>();
            repositoryMock.Setup(mock => mock.Remove(It.IsAny<Guid>()))
                .Returns(exception);
            var sut = _fixture.Initialize(repositoryMock.Object);

            var removedId = Guid.NewGuid();
            await match(sut.RemoveItemFromCart(removedId),
                Right: _  => Assert.False(true, "Should't get here!"),
                Left: l => Assert.Equal(l, expected)
            );

            repositoryMock.Verify(
                mock => mock.Remove(It.Is<Guid>(id => id == removedId)),
                Times.Once()
            );
        }
    }
}
