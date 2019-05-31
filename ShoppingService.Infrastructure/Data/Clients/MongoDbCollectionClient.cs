using System;
using System.Collections.Generic;
using MongoDB.Driver;
using ShoppingService.Core.Cart;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ShoppingService.Infrastructure.Data.Clients
{
    public class MongoDbCollectionClient<T> : IDocumentDbClient<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbCollectionClient(IMongoCollection<T> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public static MongoDbCollectionClient<T> Create(
            string connectionString, string databaseName, string collectionName
        ) {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<T>(collectionName);
            return new MongoDbCollectionClient<T>(collection);
        }

        public TryOptionAsync<T> CreateDocumentAsync(T item) =>
            TryOptionAsync(async () =>
            {
                await _collection.InsertOneAsync(item);
                return Some(item);
            });

        public TryOptionAsync<List<T>> GetDocumentsAsync(int pageNumber = 0, int pageSize = 50) =>
           TryOptionAsync(async () => await _collection.Find(item => true)
                 .Limit(pageSize).Skip(pageSize * (pageNumber))
                 .ToListAsync());

        public TryOptionAsync<T> GetDocumentByIdAsync(Func<T, bool> compareFunc) =>
            TryOptionAsync(async () =>
            {
               var result = await _collection.FindAsync(item => compareFunc(item));
               return result.First<T>();
            });

        public TryOptionAsync<T> ReplaceDocumentAsync(Func<T, bool> compareFunc, T document) =>
            TryOptionAsync(async () =>
            {
                return await _collection.FindOneAndReplaceAsync<T>((item => compareFunc(item)), document);
            });

        public TryOptionAsync<T> DeleteDocumentAsync(Func<T, bool> compareFunc) =>
            TryOptionAsync(async () => await _collection.FindOneAndDeleteAsync(item => compareFunc(item)));
    }
}
