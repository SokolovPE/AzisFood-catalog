using Catalog.DataAccess.Models;
using Catalog.Dto;

namespace Catalog.Services.Interfaces
{
    public interface IProductService : IService<Product, ProductDto, ProductRequestDto>
    {
    }
}