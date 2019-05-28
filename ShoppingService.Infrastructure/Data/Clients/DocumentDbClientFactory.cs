using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ShoppingService.Infrastructure.Data.Clients {
    public class DocumentDbClientFactory {
        public static IDocumentDbClient CreateAndConnect(Uri serviceEndpoint, string authKey, string databaseName, string collectionName) {
            var documentClient = new DocumentClient(serviceEndpoint, authKey);
            documentClient.OpenAsync().Wait();

            return new DocumentDbClient(databaseName, collectionName, documentClient);
        }
    }
}
