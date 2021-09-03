using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;

namespace Catalog.Core.Services.Interfaces
{
    /// <inheritdoc />
    public interface IProductService : IService<Product, ProductDto, ProductRequestDto>
    {
    }
}