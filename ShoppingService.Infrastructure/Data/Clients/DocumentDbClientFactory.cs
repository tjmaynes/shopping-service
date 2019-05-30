using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Data.Clients {
    public class DocumentDbClientFactory<T> {
        public static IDocumentDbClient<T> CreateAndConnect(Uri serviceEndpoint, string authKey, string databaseName, string collectionName) {
            var documentClient = new DocumentClient(serviceEndpoint, authKey);
            documentClient.OpenAsync().Wait();

            return new DocumentDbClient<T>(databaseName, collectionName, documentClient);
        }
    }
}
