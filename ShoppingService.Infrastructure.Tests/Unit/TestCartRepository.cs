using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Clients;
using ShoppingService.Infrastructure.Data.Repositories;
using ShoppingService.Infrastructure.Tests.Helpers;
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
            var expected = new List<CartItem>();
            expected.Add(new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow));
            var response = TryOptionAsync(async () => {
                await Task.Delay(100);
                return expected as IEnumerable<CartItem>;
            });

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(x => x.GetDocumentsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(response);

            var countLimit = 100;
            var sut = new CartRepository(documentDbClient.Object);
            await match<string, IEnumerable<CartItem>>(sut.GetAll(countLimit),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                x => x.GetDocumentsAsync(
                    It.Is<int>(number => number == countLimit),
                    default(CancellationToken)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Add_WhenCalled_ShouldEventually_StoreCartItem()
        {
            CartItem expected = new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = TryOptionAsync<ResourceResponse<Document>>(async () => {
                await Task.Delay(100);
                return DocumentsClientHelpers.CreateDocumentResponse(expected);
            });

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(x => x.CreateDocumentAsync(
                It.IsAny<CartItem>(), null, It.IsAny<bool>(), It.IsAny<CancellationToken>())
            ).Returns(response);

            var sut = new CartRepository(documentDbClient.Object);
            await match<string, CartItem>(sut.Add(expected),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                x => x.CreateDocumentAsync(
                    It.Is<CartItem>(item => item.Equals(expected)),
                    null,
                    false,
                    default(CancellationToken)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task GetById_WhenCalled_ShouldEventually_ReturnACartId()
        {
            var expectedId = Guid.NewGuid();
            var expected = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            var response = TryOptionAsync<ResourceResponse<Document>>(async () => {
                await Task.Delay(100);
                return DocumentsClientHelpers.CreateDocumentResponse(expected);
            });

            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(x => x.ReplaceDocumentAsync(It.IsAny<string>(), It.IsAny<CartItem>(), null, It.IsAny<CancellationToken>()))
                .Returns(response);

            var sut = new CartRepository(documentDbClient.Object);
            await match<string, CartItem>(sut.Update(expected),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                x => x.ReplaceDocumentAsync(
                    It.Is<string>(id => id == expectedId.ToString()),
                    It.Is<CartItem>(item => item.Equals(expected)),
                    null,
                    default(CancellationToken)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Update_WhenCalled_ShouldEventually_UpdateACartItem()
        {
            var expectedId = Guid.NewGuid();
            var expected = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
            
            var response = TryOptionAsync<ResourceResponse<Document>>(async () => {
                await Task.Delay(100);
                return DocumentsClientHelpers.CreateDocumentResponse(expected);
            });
            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(x => x.ReplaceDocumentAsync(
                It.IsAny<string>(), It.IsAny<CartItem>(), null, It.IsAny<CancellationToken>())
            ).Returns(response);

            var sut = new CartRepository(documentDbClient.Object);
            await match<string, CartItem>(sut.Update(expected),
                Right: r  => Assert.Equal(expected, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                x => x.ReplaceDocumentAsync(
                    It.Is<string>(id => id == expectedId.ToString()),
                    It.Is<CartItem>(item => item.Equals(expected)),
                    null,
                    default(CancellationToken)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Remove_WhenCalled_ShouldEventually_RemoveACartItemFromDb()
        {
            var expectedId = Guid.NewGuid();
            var expected = new CartItem(expectedId, "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);

            var response = TryOptionAsync<ResourceResponse<Document>>(async () => {
                await Task.Delay(100);
                return DocumentsClientHelpers.CreateDocumentResponse(expected);
            });
            var documentDbClient = new Mock<IDocumentDbClient<CartItem>>();
            documentDbClient.Setup(x => x.DeleteDocumentAsync(
                It.IsAny<string>(), null, It.IsAny<CancellationToken>())
            ).Returns(response);

            var sut = new CartRepository(documentDbClient.Object);
            await match<string, Guid>(sut.Remove(expectedId),
                Right: r  => Assert.Equal(expectedId, r),
                Left:  _  => Assert.False(true, "Shouldn't get here!")
            );

            documentDbClient.Verify(
                x => x.DeleteDocumentAsync(
                    It.Is<string>(id => id == expectedId.ToString()),
                    null,
                    default(CancellationToken)
                ),
                Times.Once
            );
        }
    }
}
