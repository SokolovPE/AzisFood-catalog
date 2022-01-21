using System;
using System.Collections.Generic;
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
        /// Get entity from database by id
        /// </summary>
        /// <param name="id">Id of entity to get</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns></returns>
        [HttpGet("Category/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto[]>> GetInCategory(Guid id, CancellationToken token)
        {
            try
            {
                return await _productService.GetProductsInCategory(id, token);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Set categories to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="categoryIds">Collection of category IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("SetCategories/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SetCategories(Guid productId, [FromBody] IEnumerable<Guid> categoryIds,
            CancellationToken token)
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
        /// <param name="categoryIds">Collection of category IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("AssignCategories/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AssignCategories(Guid productId, [FromBody] IEnumerable<Guid> categoryIds,
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
        /// <param name="categoryIds">Collection of category IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("RetainCategories/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RetainCategories(Guid productId, [FromBody] IEnumerable<Guid> categoryIds,
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

        /// <summary>
        /// Set ingredients to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="ingredientUsages">Collection of ingredient usages to be set</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("SetIngredients/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SetIngredients(Guid productId,
            [FromBody] IEnumerable<IngredientUsageDto> ingredientUsages, CancellationToken token)
        {
            try
            {
                await _productService.SetIngredients(productId, ingredientUsages, token);
                return Ok("Ingredients were successfully set");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Assign additional ingredients to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="ingredientUsages">Collection of ingredient usages to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("AssignIngredients/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AssignCategories(Guid productId,
            [FromBody] IEnumerable<IngredientUsageDto> ingredientUsages,
            CancellationToken token)
        {
            try
            {
                await _productService.AssignIngredients(productId, ingredientUsages, token);
                return Ok("Ingredients were successfully assigned");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Retain ingredients from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="ingredientIds">Collection of ingredient IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("RetainIngredients/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RetainIngredients(Guid productId,
            [FromBody] IEnumerable<Guid> ingredientIds,
            CancellationToken token)
        {
            try
            {
                await _productService.RetainIngredients(productId, ingredientIds, token);
                return Ok("Categories were successfully retained");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        /// <summary>
        /// Set options to product, will replace existing ones
        /// </summary>
        /// <param name="productId">ID of product to set to</param>
        /// <param name="optionIds">Collection of option IDs to be set</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("SetOptions/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SetOptions(Guid productId, [FromBody] IEnumerable<Guid> optionIds,
            CancellationToken token)
        {
            try
            {
                await _productService.SetOptions(productId, optionIds, token);
                return Ok("Options were successfully set");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Assign additional options to product
        /// </summary>
        /// <param name="productId">ID of product to assign to</param>
        /// <param name="optionIds">Collection of option IDs to be assigned</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("AssignOptions/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AssignOptions(Guid productId, [FromBody] IEnumerable<Guid> optionIds,
            CancellationToken token)
        {
            try
            {
                await _productService.AssignOptions(productId, optionIds, token);
                return Ok("Options were successfully assigned");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Retain options from product
        /// </summary>
        /// <param name="productId">ID of product to retain from</param>
        /// <param name="optionIds">Collection of option IDs to be retained</param>
        /// <param name="token">Token for operation cancel</param>
        [HttpPatch("RetainOptions/{productId}")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RetainOptions(Guid productId, [FromBody] IEnumerable<Guid> optionIds,
            CancellationToken token)
        {
            try
            {
                await _productService.RetainOptions(productId, optionIds, token);
                return Ok("Options were successfully retained");
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}