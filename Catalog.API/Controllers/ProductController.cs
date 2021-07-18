using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Dto;
using Catalog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns>List of <see cref="Product"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> Get()
        {
            try
            {
                var data = await _productService.GetAllAsync();
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get product from database by id.
        /// </summary>
        /// <param name="id">Id of product to get.</param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public async Task<ActionResult<ProductDto>> Get(string id)
        {
            try
            {
                return await _productService.GetByIdAsync(id);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="product">Model of new product.</param>
        /// <returns>Model of product, created on the database.</returns>
        [HttpPost]
        public async Task<ActionResult<Product>> Create(ProductRequestDto product)
        {
            try
            {
                var insertedProduct = await _productService.AddAsync(product);
                return CreatedAtRoute("GetProduct", new {id = insertedProduct.Id},
                    insertedProduct);
            }
            catch (InvalidConstraintException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Update product in database.
        /// </summary>
        /// <param name="id">Identifier of product.</param>
        /// <param name="productIn">New model of product.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductRequestDto productIn)
        {
            try
            {
                await _productService.UpdateAsync(id, productIn);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Delete product from database.
        /// </summary>
        /// <param name="id">Identifier of product.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var product = await _productService.DeleteAsync(id);
                return Ok($"Successfully deleted {product.Title} [{product.Id}]");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Delete product from database.
        /// </summary>
        /// <param name="ids">Array of product identifiers.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpDelete("DeleteMany")]
        public async Task<IActionResult> DeleteMany([FromBody] string[] ids)
        {
            await _productService.DeleteAsync(ids);
            return Ok($"Successfully deleted {ids.Length} product(s)");
        }
    }
}