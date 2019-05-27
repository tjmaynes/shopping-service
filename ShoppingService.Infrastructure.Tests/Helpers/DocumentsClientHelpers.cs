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
            var document = CreateDocumentFromObject(item);
            return new ResourceResponse<Document>(document);
        }

        public static ResourceResponse<Document> CreateDocumentsResponse(IEnumerable<object> items)
        {
            var document = CreateDocumentFromObjects(items);
            return new ResourceResponse<Document>(document);
        }

        public static Document CreateDocumentFromObjects(IEnumerable<object> items) {
            var json = JsonConvert.SerializeObject(items);
            return CreateDocumentFromJSON(json);
        }

        public static Document CreateDocumentFromObject(object item) {
            var json = JsonConvert.SerializeObject(item);
            return CreateDocumentFromJSON(json);
        }

        public static Document CreateDocumentFromJSON(string json) {
            var jsonReader = new JsonTextReader(new StringReader(json));
            var document = new Document();
            document.LoadFrom(jsonReader);
            return document;
        }
    }
}
