using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace ShoppingService.Infrastructure.Tests.Helpers
{
    public class DocumentsClientHelpers
    {
        public static ResourceResponse<Document> CreateDocumentResponse(Object item)
        {
            var documentJson = JsonConvert.SerializeObject(item);
            return CreateResourceResponse(documentJson);
        }

        public static ResourceResponse<Document> CreateDocumentsResponse(IEnumerable<object> items)
        {
            var documentJson = JsonConvert.SerializeObject(items);
            return CreateResourceResponse(documentJson);
        }

        private static ResourceResponse<Document> CreateResourceResponse(string jsonData) {
            var jsonReader = new JsonTextReader(new StringReader(jsonData));

            var document = new Document();
            document.LoadFrom(jsonReader);

            return new ResourceResponse<Document>(document);
        }
    }
}
