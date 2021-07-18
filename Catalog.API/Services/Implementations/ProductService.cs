using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Dto;
using Catalog.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalog.Services.Implementations
{
    public class ProductService : BaseService<Product, ProductDto, ProductRequestDto>, IProductService
    {
        public ProductService(ILogger<ProductService> logger, IMapper mapper, ICachedBaseRepository<Product> repository) 
            : base(logger, mapper, repository)
        {
        }
    }
}