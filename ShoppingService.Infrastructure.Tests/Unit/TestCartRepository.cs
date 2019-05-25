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

namespace ShoppingService.Infrastructure.Tests.Unit
{

    public class TestCartRepository
    {
        // [Fact]
        // public async Task CartRepository_Add_ShouldStoreCartItem()
        // {
        //     var cartItem = new CartItem(Guid.NewGuid(), "some-name-1", 45.99m, "some-manufacturer", DateTime.UtcNow);
        //     var newDocument = DocumentsClientHelpers.CreateDocumentResponse(cartItem);
        //     var documentDbClient = new Mock<IDocumentDbClient>();
        //     documentDbClient.Setup(x => x.CreateDocumentAsync(It.IsAny<Uri>(), null))
        //         .ReturnsAsync(newDocument);

        //     var _sut = new CartRepository(documentDbClient.Object);
        //     var _ = await _sut.Add(cartItem);

        //     documentDbClient.Verify(
        //         x => x.CreateDocumentAsync(
        //             It.Is<Uri>(uri => uri == UriFactory.CreateDocumentCollectionUri("test-cart-items-db", "cart-items")),
        //             It.Is<object>(document => document == newDocument),
        //             false,
        //             default(CancellationToken)
        //         ),
        //         Times.Once
        //     );
        // }
    }
}
