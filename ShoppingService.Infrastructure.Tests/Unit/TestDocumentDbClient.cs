using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Linq.Expressions;
using ShoppingService.Core.Cart;
using ShoppingService.Core.Common;
using ShoppingService.Infrastructure.Data.Clients;
using ShoppingService.Infrastructure.Tests.Fixtures;
using ShoppingService.Infrastructure.Tests.Helpers;
using Xunit;
using Moq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Tests.Unit
{
    public class TestDocumentDbClient : IClassFixture<DocumentDbClientFixture>
    {
        // Issue1 => https://stackoverflow.com/a/48432085
        // Issue2 => https://stackoverflow.com/q/48349659
        public interface IFakeDocumentQuery<T> : IDocumentQuery<T>, IOrderedQueryable<T> { }

        private readonly DocumentDbClientFixture _fixture;

        public TestDocumentDbClient(DocumentDbClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DocumentDbClient_WithNoNullArguments_ShouldReturnDocumentDbClient()
        {
            var documentClientMock = new Mock<IDocumentClient>();
            var sut = _fixture.Initialize(documentClientMock.Object);
            Assert.NotNull(sut);
        }

        [Theory]
        [InlineData(null, null, null, "databaseName")]
        [InlineData("some-database-name", null, null, "collectionName")]
        [InlineData("some-database-name", "some-collection-name", null, "documentClient")]
        public void DocumentDbClient_WithNullArgument_ShouldThrowNullException(string databaseName,
            string collectionName, IDocumentClient documentClient, string paramName)
        {
            var except = Assert.Throws<ArgumentNullException>(() =>
                new DocumentDbClient<object>(databaseName, collectionName, documentClient));
            Assert.Equal(paramName, except.ParamName);
        }

        [Fact]
        public async Task CreateDatabaseAsync_WhenCalled_ShouldEventually_CreateDatabaseUsingDocumentClient()
        {
            var expected = new Database { Id = _fixture.DatabaseName };
            var response = new ResourceResponse<Database>(expected);
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(mock => mock.CreateDatabaseIfNotExistsAsync(It.IsAny<Database>(), null))
                .ReturnsAsync(response);

            var sut = _fixture.Initialize(documentClientMock.Object);
            var result = await match(sut.CreateDatabaseAsync(),
                Right: _ => "Success!",
                Left: ex => ex.Message
            );

            documentClientMock.Verify(
                mock => mock.CreateDatabaseIfNotExistsAsync(
                    It.Is<Database>(database => database.Id == expected.Id),
                    null
               ),
                Times.Once()
            );

            Assert.Equal("Success!", result);
        }

        [Fact]
        public async Task CreateDocumentAsync_WhenCalled_ShouldEventually_CreateNewDocumentUsingDocumentClient()
        {
            var item = new { ID = Guid.NewGuid(), FirstName = "some-first-name", LastName = "some-last-name" };
            var expected = DocumentsClientHelpers.CreateDocumentResponse(item);
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(mock => mock.CreateDocumentAsync(
                It.IsAny<Uri>(), It.IsAny<object>(), null, false,
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expected);

            var sut = _fixture.Initialize(documentClientMock.Object);
            var result = await match(sut.CreateDocumentAsync(item),
                Right: _ => "Success!",
                Left: ex => ex.Message
            );
 
            documentClientMock.Verify(
                mock => mock.CreateDocumentAsync(
                    It.Is<Uri>(uri =>
                        uri == UriFactory.CreateDocumentCollectionUri("some-database-name", "some-collection-name")),
                    It.Is<object>(document => document == item),
                    null,
                    false,
                    default(CancellationToken)
                ),
                Times.Once()
            );

            Assert.Equal("Success!", result);
        }

        [Fact]
        public async Task GetDocumentsAsync_WhenCalled_ShouldEventually_GetAllDocumentsUsingDocumentClient()
        {
            List<object> expected = new List<object>();
            expected.Add(new { ID = Guid.NewGuid(), FirstName = "some-first-name", LastName = "some-last-name" });
            FeedResponse<object> response = new FeedResponse<object>(expected);

            var documentQueryMock = new Mock<IFakeDocumentQuery<Document>>();
            documentQueryMock.SetupSequence(mock => mock.HasMoreResults)
                .Returns(true)
                .Returns(false);
            documentQueryMock.Setup(mock => mock.ExecuteNextAsync<object>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(mock => mock.CreateDocumentQuery<Document>(It.IsAny<Uri>(), It.IsAny<FeedOptions>()))
                .Returns(documentQueryMock.Object);

            var sut = _fixture.Initialize(documentClientMock.Object);
            var result = await match(sut.GetDocumentsAsync(),
                Right: _ => "Success!",
                Left: ex => ex.Message
            );

            documentQueryMock.Verify(
                mock => mock.ExecuteNextAsync<object>(default(CancellationToken)),
                Times.Once()
            );

            documentClientMock.Verify(
                mock => mock.CreateDocumentQuery<Document>(
                    It.Is<Uri>(uri =>
                        uri == UriFactory.CreateDocumentCollectionUri("some-database-name", "some-collection-name")),
                    It.Is<FeedOptions>(options => options.MaxItemCount == 200)
                ),
                Times.Once()
            );

            Assert.Equal("Success!", result);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_WhenCalled_ShouldEventually_GetDocumentUsingDocumentClient()
        {
            var id = Guid.NewGuid();
            var item = new { ID = id, FirstName = "some-first-name", LastName = "some-last-name" };
            var expected = DocumentsClientHelpers.CreateDocumentResponse(item);
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(mock => mock.ReadDocumentAsync(It.IsAny<Uri>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var sut = _fixture.Initialize(documentClientMock.Object);
            var result = await match(sut.GetDocumentByIdAsync(id.ToString()),
                Right: _ => "Success!",
                Left: ex => ex.Message
            );

            documentClientMock.Verify(
                mock => mock.ReadDocumentAsync(
                    It.Is<Uri>(uri =>
                        uri == UriFactory.CreateDocumentUri("some-database-name", "some-collection-name", id.ToString())),
                    null,
                    default(CancellationToken)
                ),
                Times.Once()
            );

            Assert.Equal("Success!", result);
        }

        [Fact]
        public async Task ReplaceDocumentAsync_WhenCalled_ShouldEventually_ReplaceDocumentUsingDocumentClient()
        {
            var id = Guid.NewGuid();
            var item = new { ID = id, FirstName = "some-first-name", LastName = "some-last-name-2" };
            var expected = DocumentsClientHelpers.CreateDocumentResponse(item);
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(mock => mock.ReplaceDocumentAsync(
                It.IsAny<Uri>(), It.IsAny<object>(), null,
                It.IsAny<CancellationToken>())
            )
                .ReturnsAsync(expected);

            var sut = _fixture.Initialize(documentClientMock.Object);
            var result = await match(sut.ReplaceDocumentAsync(id.ToString(), item),
                Right: _ => "Success!",
                Left: ex => ex.Message
            );

            documentClientMock.Verify(
                mock => mock.ReplaceDocumentAsync(
                   It.Is<Uri>(uri =>
                        uri == UriFactory.CreateDocumentUri("some-database-name", "some-collection-name", id.ToString())),
                    It.Is<object>(document => document == item),
                    null,
                    default(CancellationToken)
                ),
                Times.Once()
            );

            Assert.Equal("Success!", result);
        }

        [Fact]
        public async Task DeleteDocumentAsync_WhenCalled_ShouldEventually_DeleteDocumenByIdtUsingDocumentClient() {
            var id = Guid.NewGuid();
            var item = new { ID = id, FirstName = "some-first-name", LastName = "some-last-name-2" };
            var expected = DocumentsClientHelpers.CreateDocumentResponse(item);
            var documentClientMock = new Mock<IDocumentClient>();
            documentClientMock.Setup(mock => mock.DeleteDocumentAsync(It.IsAny<Uri>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var sut = _fixture.Initialize(documentClientMock.Object);
            var result = await match(sut.DeleteDocumentAsync(id.ToString()),
                Right: _ => "Success!",
                Left: ex => ex.Message
            );

            documentClientMock.Verify(
                mock => mock.DeleteDocumentAsync(
                   It.Is<Uri>(uri =>
                        uri == UriFactory.CreateDocumentUri("some-database-name", "some-collection-name", id.ToString())),
                    null,
                    default(CancellationToken)
                ),
                Times.Once()
            );

            Assert.Equal("Success!", result);
        }
    }
}
