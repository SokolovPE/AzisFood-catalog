using Catalog.DataAccess.Models;
using Catalog.Dto;

namespace Catalog.Services.Interfaces
{
    public interface IProductService : IAbstractService<Product, ProductDto, ProductRequestDto>
    {
    }
}