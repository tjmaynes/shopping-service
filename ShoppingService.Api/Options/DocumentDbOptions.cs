using System.Collections.Generic;

namespace ShoppingService.Api.Options {
    public class DocumentDbOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public Dictionary<string, string> CollectionNames { get; set; }

        public void Deconstruct(out string connectionString, out string databaseName, out Dictionary<string, string> collectionNames)
        {
            connectionString = ConnectionString;
            databaseName = DatabaseName;
            collectionNames = CollectionNames;
        }
    }
}
