using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.Core.Services.Interfaces;
using Catalog.Sdk.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Catalog.Controllers
{
    /// <summary>
    /// Base implementation of controller
    /// </summary>
    /// <typeparam name="T">Main type</typeparam>
    /// <typeparam name="TDto">Dto of main type</typeparam>
    /// <typeparam name="TRequestDto">Request Dto of main type</typeparam>
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseController<T, TDto, TRequestDto> : ControllerBase
        where T: IRepoEntity 
        where TDto: IDto
        where TRequestDto: IRequestDto
    {
        /// <summary>
        /// Log operator
        /// </summary>
        protected readonly ILogger<BaseController<T, TDto, TRequestDto>> ControllerLogger;
        private readonly IService<T, TDto, TRequestDto> _service;
        
        /// <summary>
        /// Constructs base controller
        /// </summary>
        /// <param name="controllerLogger">Logger service</param>
        /// <param name="service">Service to work with entity</param>
        protected BaseController(ILogger<BaseController<T, TDto, TRequestDto>> controllerLogger, IService<T, TDto, TRequestDto> service)
        {
            ControllerLogger = controllerLogger;
            _service = service;
        }
        
        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>List of Dto of entity</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TDto>>> Get(CancellationToken token)
        {
            try
            {
                var data = await _service.GetAllAsync(token);
                return data;
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        /// <summary>
        /// Get entity from database by id
        /// </summary>
        /// <param name="id">Id of entity to get</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TDto>> Get(Guid id, CancellationToken token)
        {
            try
            {
                return await _service.GetByIdAsync(id, token);
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
        /// Create a new entity
        /// </summary>
        /// <param name="item">Model of new entity</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Model of entity, created on the database</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<T>> Create(TRequestDto item, CancellationToken token)
        {
            try
            {
                var insertedItem = await _service.AddAsync(item, token);
                return Created(Request.GetDisplayUrl(), insertedItem);
            }
            catch (InvalidConstraintException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        /// <summary>
        /// Update entity in database
        /// </summary>
        /// <param name="id">Identifier of entity</param>
        /// <param name="itemIn">New model of entity</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Indicates success or fail of operation</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] TRequestDto itemIn, CancellationToken token)
        {
            try
            {
                await _service.UpdateAsync(id, itemIn, token);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="id">Identifier of entity</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Indicates success or fail of operation</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            try
            {
                var item = await _service.DeleteAsync(id, token);
                return Ok($"Successfully deleted [{JsonConvert.SerializeObject(item)}]");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                ControllerLogger.LogError(e, e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
        
        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="ids">Array of entity identifiers</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Indicates success or fail of operation</returns>
        [HttpDelete("DeleteMany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMany([FromBody] Guid[] ids, CancellationToken token)
        {
            await _service.DeleteAsync(ids, token);
            return Ok($"Successfully deleted {ids.Length} item(s)");
        }
    }
}