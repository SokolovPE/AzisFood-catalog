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
    public class ProductService : IProductService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;
        private readonly ICachedBaseRepository<Product> _productRepository;
        public ProductService(ILogger<ProductService> logger, IMapper mapper, ICachedBaseRepository<Product> productRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _productRepository = productRepository;
        }
        
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAsync();
                return _mapper.Map<List<ProductDto>>(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception during attempt to get all product from catalog");
                throw;
            }
        }

        public async Task<ProductDto> GetProductByIdAsync(string id)
        {
            var product = await _productRepository.GetAsync(id);

            if (product != null) return _mapper.Map<ProductDto>(product);
            
            var msg = $"Requested product with id: {id} was not found in catalog.";
            _logger.LogWarning(msg);
            throw new KeyNotFoundException(msg);
        }

        public async Task<ProductDto> AddProductAsync(ProductRequestDto product)
        {
            try
            {
                var productToInsert = _mapper.Map<Product>(product);
                var insertedProduct = await _productRepository.CreateAsync(productToInsert);
                return _mapper.Map<ProductDto>(insertedProduct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception during attempt to insert record");
                throw;
            }
        }

        public async Task UpdateProductAsync(string id, ProductRequestDto product)
        {
            var productFromDb = await _productRepository.GetAsync(id);

            if (productFromDb == null)
            {
                _logger.LogWarning($"Requested product for update with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            var productUpdated = _mapper.Map<Product>(product);
            productUpdated.Id = id;
            await _productRepository.UpdateAsync(id, productUpdated);
        }

        public async Task<Product> DeleteProductAsync(string id)
        {
            var product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                _logger.LogWarning($"Requested product for delete with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            await _productRepository.RemoveAsync(product.Id);
            return product;
        }

        public async Task DeleteProductsAsync(string[] ids)
        {
            if (ids.Length > 0) await _productRepository.RemoveManyAsync(ids);
        }
    }
}