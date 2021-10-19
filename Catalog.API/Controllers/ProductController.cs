using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    /// <summary>
    /// Controller to operate products
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : BaseController<Product, ProductDto, ProductRequestDto>
    {
        /// <summary>
        /// Service to be used to work with entity
        /// </summary>
        private readonly IProductService _productService;
        
        /// <inheritdoc />
        public ProductController(ILogger<ProductController> controllerLogger,
            IProductService service) : base(controllerLogger,
            service)
        {
            _productService = service;
        }

        /// <summary>
        /// Set categories to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="categoryIds">Array of category IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPost("SetCategories")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SetCategories(string productId, IEnumerable<string> categoryIds, CancellationToken token)
        {
            try
            {
                await _productService.SetCategories(productId, categoryIds, token);
                return Ok("Categories were successfully set");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Assign additional categories to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="categoryIds">Array of category IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPost("AssignCategories")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AssignCategories(string productId, IEnumerable<string> categoryIds,
            CancellationToken token)
        {
            try
            {
                await _productService.AssignCategories(productId, categoryIds, token);
                return Ok("Categories were successfully assigned");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Retain categories from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="categoryIds">Array of category IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPost("RetainCategories")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RetainCategories(string productId, IEnumerable<string> categoryIds,
            CancellationToken token)
        {
            try
            {
                await _productService.RetainCategories(productId, categoryIds, token);
                return Ok("Categories were successfully retained");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}