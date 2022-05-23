using System.Linq;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.DataAccess.Models;
using HotChocolate.Data;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Resolvers;

namespace Catalog.GraphQL;

/// <summary>
///     Catalog graph query
/// </summary>
public class CatalogQuery
{
    private readonly IBaseQueryableRepository<Category> _repository;

    /// <summary>
    ///     .ctor
    /// </summary>
    public CatalogQuery(IBaseQueryableRepository<Category> repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Category query
    /// </summary>
    [UseProjection]
    [UseFiltering]
    public IQueryable<Category> Categories(/*IResolverContext resolverContext*/)
    {
        // Need to be passed through GRPC somehow...
        // return _repository.GetQueryable().Filter(resolverContext);
        return _repository.GetQueryable();
    }
}