using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.DataAccess.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Catalog.GraphQL;

/// <summary>
///     GraphQL type description of product
/// </summary>
public class ProductType : ObjectType<Product>
{
    /// <inheritdoc />
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Description("Represents products in catalog");

        descriptor
            .Field(product => product.CategoryId)
            .ResolveWith<Resolvers>(resolvers => resolvers.GetCategories(default!, default!))
            .Description("List of categories assigned to product");
        descriptor
            .Field(product => product.OptionId)
            .ResolveWith<Resolvers>(resolvers => resolvers.GetOptions(default!, default!))
            .Description("List of options assigned to product");
    }

    private class Resolvers
    {
        public Task<IEnumerable<Category>> GetCategories([Parent] Product product,
            [Service] ICachedBaseRepository<Category> repository)
        {
            return repository.GetHashAsync(category => product.CategoryId.Contains(category.Id));
        }

        public Task<IEnumerable<Option>> GetOptions([Parent] Product product,
            [Service] ICachedBaseRepository<Option> repository)
        {
            return product.OptionId.Length <= 0
                ? default
                : repository.GetHashAsync(category => product.OptionId.Contains(category.Id));
        }
    }
}