using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Catalog.DataAccess.Models;
using Catalog.Dto;
using Catalog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    /// <summary>
    /// Base implementation of controller.
    /// </summary>
    /// <typeparam name="T">Main type.</typeparam>
    /// <typeparam name="TDto">Dto of main type.</typeparam>
    /// <typeparam name="TRequestDto">Request Dto of main type.</typeparam>
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseController<T, TDto, TRequestDto> : ControllerBase
        where T: IRepoEntity 
        where TDto: IDto
        where TRequestDto: IRequestDto
    {
        private readonly ILogger<BaseController<T, TDto, TRequestDto>> _logger;
        private readonly IService<T, TDto, TRequestDto> _service;
        
        /// <summary>
        /// Constructs base controller.
        /// </summary>
        /// <param name="logger">Logger service.</param>
        /// <param name="service">Service to work with entity.</param>
        protected BaseController(ILogger<BaseController<T, TDto, TRequestDto>> logger, IService<T, TDto, TRequestDto> service)
        {
            _logger = logger;
            _service = service;
        }
        
        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>List of Dto of entity.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TDto>>> Get()
        {
            try
            {
                var data = await _service.GetAllAsync();
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Get entity from database by id.
        /// </summary>
        /// <param name="id">Id of entity to get.</param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}", Name = "GetOne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TDto>> Get(string id)
        {
            try
            {
                return await _service.GetByIdAsync(id);
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
        /// Create a new entity.
        /// </summary>
        /// <param name="item">Model of new entity.</param>
        /// <returns>Model of entity, created on the database.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<T>> Create(TRequestDto item)
        {
            try
            {
                var insertedItem = await _service.AddAsync(item);
                return CreatedAtRoute("GetOne", new {id = insertedItem.Id},
                    insertedItem);
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
        /// Update entity in database.
        /// </summary>
        /// <param name="id">Identifier of entity.</param>
        /// <param name="itemIn">New model of entity.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] TRequestDto itemIn)
        {
            try
            {
                await _service.UpdateAsync(id, itemIn);
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
        /// Delete entity from database.
        /// </summary>
        /// <param name="id">Identifier of entity.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var item = await _service.DeleteAsync(id);
                return Ok($"Successfully deleted [{item.Id}]");
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
        /// Delete entity from database.
        /// </summary>
        /// <param name="ids">Array of entity identifiers.</param>
        /// <returns>Indicates success or fail of operation.</returns>
        [HttpDelete("DeleteMany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMany([FromBody] string[] ids)
        {
            await _service.DeleteAsync(ids);
            return Ok($"Successfully deleted {ids.Length} item(s)");
        }
    }
}