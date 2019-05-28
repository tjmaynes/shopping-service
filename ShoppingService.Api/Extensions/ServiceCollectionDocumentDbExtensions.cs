using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShoppingService.Infrastructure.Data.Clients;

namespace ShoppingService.Api.Extensions
{
    public static class ServiceCollectionDocumentDbExtensions
    {
        public static IServiceCollection AddDocumentDb(this IServiceCollection services, Uri serviceEndpoint,
            string authKey, string databaseName, List<string> collectionNames)
        {
            services.AddSingleton<IDocumentDbClient>(DocumentDbClientFactory.CreateAndConnect(
                serviceEndpoint, authKey, databaseName, collectionNames[0]
            ));
            return services;
        }
    }
}
