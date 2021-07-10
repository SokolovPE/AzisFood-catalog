using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IBaseRepository<Product> _productRepository;

        public ProductController(ILogger<ProductController> logger, IBaseRepository<Product> productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns>List of <see cref="Product"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get() =>
            await _productRepository.GetAsync();
        
        /// <summary>
        /// Get product from database by id.
        /// </summary>
        /// <param name="id">Id of product to get.</param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> Get(string id)
        {
            var product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
        
        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="product">Model of new product.</param>
        /// <returns>Model of product, created on the database.</returns>
        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            try
            {
                var insertedId = await _productRepository.CreateAsync(product);
                return CreatedAtRoute("GetProduct", new {id = insertedId.Id}, product);
            }
            catch (InvalidConstraintException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        
        /// <summary>
        /// Update product in database.
        /// </summary>
        /// <param name="id">Identifier of product.</param>
        /// <param name="productIn">New model of product.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] Product productIn)
        {
            var product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.UpdateAsync(id, productIn);

            return NoContent();
        }
        
        /// <summary>
        /// Delete product from database.
        /// </summary>
        /// <param name="id">Identifier of product.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productRepository.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.RemoveAsync(product.Id);

            return Ok($"Successfully deleted {product.Title} [{product.Id}]");
        }
        
        /// <summary>
        /// Delete product from database.
        /// </summary>
        /// <param name="ids">Array of product identifiers.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpDelete("DeleteMany")]
        public async Task<IActionResult> DeleteMany([FromBody] string[] ids)
        {
            await _productRepository.RemoveManyAsync(ids);
            return Ok($"Successfully deleted");
        }
    }
}