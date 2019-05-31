using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Clients;
using ShoppingService.Infrastructure.Data.Repositories;
using Xunit;
using Moq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Tests.Unit
{

    public class TestCartRepository
    {
        [Fact]
        public void CartRepository_WithNoNullArguments_ShouldReturnDocumentDbClient()
        {
            var dbClientMock = new Mock<IDocumentDbClient<CartItem>>();
            var sut = new CartRepository(dbClientMock.Object);
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(null, "dbClient")]
        public void CartRepository_WithNullArgument_ShouldThrowNullException(IDocumentDbClient<CartItem> dbClient, string paramName)
        {
            var except = Assert.Throws<ArgumentNullException>(() => new CartRepository(dbClient));
            Assert.Equal(paramName, except.ParamName);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ShouldEventually_FetchAllCartItems()
        {
            var data = new List<CartItem>();
            data.Add(new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            var response = TryOptionAsync<List<CartItem>>(() => Task.Run(() => data));

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.GetDocumentsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(response);

            var sut = new CartRepository(documentDbClient.Object);
            var expected = Some(new PagedResult<CartItem>(
                data, 1, 1, 1
            ));

            await match<Exception, Option<PagedResult<CartItem>>>(sut.GetAll(0, 1),
                Right: result => Assert.Equal(expected, result),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                mock => mock.GetDocumentsAsync(
                    It.Is<int>(number => number == 0),
                    It.Is<int>(number => number == 1)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAll_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnException()
        {
            var expected = new Exception("Unknown error occurred!");
            var response = TryOptionAsync<List<CartItem>>(() => throw expected);

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.GetDocumentsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(response);

            var sut = new CartRepository(documentDbClient.Object);

            await match<Exception, Option<PagedResult<CartItem>>>(sut.GetAll(0, 1),
                Right:  _  => Assert.False(true, "Shouldn't get here!"),
                Left: ex  => Assert.Equal(expected, ex)
            );

            documentDbClient.Verify(
                mock => mock.GetDocumentsAsync(
                    It.Is<int>(number => number == 0),
                    It.Is<int>(number => number == 1)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Add_WhenCalled_ShouldEventually_StoreCartItem()
        {
            var newItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = TryOptionAsync<CartItem>(() => Task.Run(() => newItem));

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.CreateDocumentAsync(It.IsAny<CartItem>()))
                .Returns(response);

            var expected = Some(newItem);

            var sut = new CartRepository(documentDbClient.Object);
            await match<Exception, Option<CartItem>>(sut.Add(newItem),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                mock => mock.CreateDocumentAsync(It.Is<CartItem>(item => item.Equals(newItem))),
                Times.Once
            );
        }


        [Fact]
        public async Task Add_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnException()
        {
            var newItem = new CartItem(Guid.NewGuid().ToString(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = new Exception("Unknown error occurred!");
            var response = TryOptionAsync<CartItem>(() => throw expected);

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.CreateDocumentAsync(It.IsAny<CartItem>()))
                .Returns(response);

            var sut = new CartRepository(documentDbClient.Object);
            await match<Exception, Option<CartItem>>(sut.Add(newItem),
                Right: _  => Assert.False(true, "Shouldn't get here!"),
                Left: ex => Assert.Equal(expected, ex)
            );

            documentDbClient.Verify(
                mock => mock.CreateDocumentAsync(It.Is<CartItem>(item => item.Equals(newItem))),
                Times.Once
            );
        }

        [Fact]
        public async Task GetById_WhenCalled_ShouldEventually_ReturnCartItem()
        {
            var expectedId = Guid.NewGuid().ToString();
            var item = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = TryOptionAsync<CartItem>(() => Task.Run(() => item));

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.GetDocumentByIdAsync(It.IsAny<Func<CartItem, bool>>()))
                .Returns(response);
            var sut = new CartRepository(documentDbClient.Object);

            var expected = Some(item);
            await match<Exception, Option<CartItem>>(sut.GetById(expectedId),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(mock => mock.GetDocumentByIdAsync(It.IsAny<Func<CartItem, bool>>()), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnException()
        {
            var expectedId = Guid.NewGuid().ToString();
            var item = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = new Exception("Unknown error occurred!");
            var response = TryOptionAsync<CartItem>(() => throw expected);

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.GetDocumentByIdAsync(It.IsAny<Func<CartItem, bool>>()))
                .Returns(response);
            var sut = new CartRepository(documentDbClient.Object);

            await match<Exception, Option<CartItem>>(sut.GetById(expectedId),
                Right: _  => Assert.False(true, "Shouldn't get here!"),
                Left: ex => Assert.Equal(expected, ex)
            );

            documentDbClient.Verify(mock => mock.GetDocumentByIdAsync(It.IsAny<Func<CartItem, bool>>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCalled_ShouldEventually_UpdateACartItem()
        {
            var expectedId = Guid.NewGuid().ToString();
            var updatedItem = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = TryOptionAsync<CartItem>(() => Task.Run(() => updatedItem));

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.ReplaceDocumentAsync(It.IsAny<Func<CartItem, bool>>(), It.IsAny<CartItem>()))
                .Returns(response);
            var sut = new CartRepository(documentDbClient.Object);

            var expected = Some(updatedItem);

            await match<Exception, Option<CartItem>>(sut.Update(updatedItem),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                mock => mock.ReplaceDocumentAsync(
                    It.IsAny<Func<CartItem, bool>>(),
                    It.Is<CartItem>(item => item.Equals(updatedItem))
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnException()
        {
            var expectedId = Guid.NewGuid().ToString();
            var updatedItem = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var expected = new Exception("Unknown error occurred!");
            var response = TryOptionAsync<CartItem>(() => throw expected);

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.ReplaceDocumentAsync(It.IsAny<Func<CartItem, bool>>(), It.IsAny<CartItem>()))
                .Returns(response);
            var sut = new CartRepository(documentDbClient.Object);

            await match<Exception, Option<CartItem>>(sut.Update(updatedItem),
                Right:  _  => Assert.False(true, "Shouldn't get here!"),
                Left: ex => Assert.Equal(expected, ex)
            );

            documentDbClient.Verify(
                mock => mock.ReplaceDocumentAsync(
                    It.IsAny<Func<CartItem, bool>>(),
                    It.Is<CartItem>(item => item.Equals(updatedItem))
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Remove_WhenCalled_ShouldEventually_RemoveACartItemFromDb()
        {
            var expectedId = Guid.NewGuid().ToString();
            var updatedItem = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = TryOptionAsync<CartItem>(() => Task.Run(() => updatedItem));

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.DeleteDocumentAsync(It.IsAny<Func<CartItem, bool>>()))
                .Returns(response);
            var sut = new CartRepository(documentDbClient.Object);

            var expected = Some(updatedItem);

            await match<Exception, Option<CartItem>>(sut.Remove(expectedId),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(mock => mock.DeleteDocumentAsync(It.IsAny<Func<CartItem, bool>>()), Times.Once);
        }

        [Fact]
        public async Task Remove_WhenCalled_AndExceptionOccurs_ShouldEventually_ReturnException()
        {
            var expectedId = Guid.NewGuid().ToString();
            var updatedItem = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);;
            var expected = new Exception("Unknown error occurred!");
            var response = TryOptionAsync<CartItem>(() => throw expected);

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(mock => mock.DeleteDocumentAsync(It.IsAny<Func<CartItem, bool>>()))
                .Returns(response);
            var sut = new CartRepository(documentDbClient.Object);

            await match<Exception, Option<CartItem>>(sut.Remove(expectedId),
                Right:  _  => Assert.False(true, "Shouldn't get here!"),
                Left: ex => Assert.Equal(expected, ex)
            );

            documentDbClient.Verify(mock => mock.DeleteDocumentAsync(It.IsAny<Func<CartItem, bool>>()), Times.Once);
        }
    }
}
