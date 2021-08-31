using Catalog.DataAccess.Models;
using Catalog.Dto;

namespace Catalog.Services.Interfaces
{
    /// <inheritdoc />
    public interface IProductService : IService<Product, ProductDto, ProductRequestDto>
    {
    }
}