using Catalog.GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Extensions;

/// <summary>
///     Extensions for dependency injection
/// </summary>
// ReSharper disable once InconsistentNaming
public static class DIExtensions
{
    /// <summary>
    ///     Add GraphQL to application
    /// </summary>
    /// <param name="serviceCollection">Collection of services</param>
    // ReSharper disable once InconsistentNaming
    public static void AddGraphQL(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddType<ProductType>()
            .AddFiltering()
            .AddSorting()
            .AddMongoDbFiltering()
            .AddMongoDbSorting()
            .AddMongoDbProjections();
    }
}