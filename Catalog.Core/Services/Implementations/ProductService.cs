using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Core.Services.Implementations
{
    /// <summary>
    /// Service to operate products.
    /// </summary>
    public class ProductService : BaseService<Product, ProductDto, ProductRequestDto>, IProductService
    {
        private readonly IValidatorService<Product> _validator;

        /// <inheritdoc />
        public ProductService(ILogger<ProductService> logger,
            IMapper mapper,
            ICachedBaseRepository<Product> repository,
            IValidatorService<Product> validator) 
            : base(logger, mapper, repository)
        {
            _validator = validator;
        }

        /// <inheritdoc />
        public override async Task<ProductDto> AddAsync(ProductRequestDto item)
        {
            try
            {
                var itemToInsert = Mapper.Map<Product>(item);
                var (validationResult, validationMessage) = await _validator.Validate(itemToInsert);
                if (!validationResult)
                {
                    throw new ValidationException(validationMessage);
                }
                var insertedItem = await Repository.CreateAsync(itemToInsert);
                return Mapper.Map<ProductDto>(insertedItem);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception during attempt to insert record of {EntityName}");
                throw;
            }
        }
    }
}