using System.Collections.Generic;

namespace ShoppingService.Api.Options {
    public class DocumentDbOptions
    {
        public string DatabaseName { get; set; }
        public Dictionary<string, string> CollectionNames { get; set; }

        public void Deconstruct(out string databaseName, out Dictionary<string, string> collectionNames)
        {
            databaseName = DatabaseName;
            collectionNames = CollectionNames;
        }
    }
}
