using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.Core.Services.Interfaces;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Core.Services.Implementations
{
    /// <summary>
    /// Basic service to operate entity
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <typeparam name="TDto">Type of entity Dto</typeparam>
    /// <typeparam name="TRequestDto">Type of entity request Dto</typeparam>
    public class BaseService<T, TDto, TRequestDto> : IService<T, TDto, TRequestDto>
        where T: class, IRepoEntity
        where TDto: IDto
        where TRequestDto: IRequestDto
    {
        /// <summary>
        /// Logger service
        /// </summary>
        protected readonly ILogger<BaseService<T, TDto, TRequestDto>> Logger;
        
        /// <summary>
        /// Automapper
        /// </summary>
        protected readonly IMapper Mapper;
        
        /// <summary>
        /// Entity repository
        /// </summary>
        protected readonly ICachedBaseRepository<T> Repository;
        
        /// <summary>
        /// Name of entity
        /// </summary>
        protected readonly string EntityName;

        /// <summary>
        /// Constructs BaseService
        /// </summary>
        /// <param name="logger">Logger service</param>
        /// <param name="mapper">Automapper</param>
        /// <param name="repository">Entity repository</param>
        public BaseService(ILogger<BaseService<T, TDto, TRequestDto>> logger,
            IMapper mapper,
            ICachedBaseRepository<T> repository)
        {
            Logger = logger;
            Mapper = mapper;
            Repository = repository;
            EntityName = typeof(T).Name;
        }
        
        /// <summary>
        /// Get whole collection of entity
        /// </summary>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>List of element dto</returns>
        public virtual async Task<List<TDto>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                var items = await Repository.GetHashAsync(token);
                return Mapper.Map<List<TDto>>(items);
            }
            catch (OperationCanceledException)
            {
                // Throw cancelled operation, do not catch
                throw;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception during attempt to get all {EntityName} from catalog");
                throw;
            }
        }

        /// <summary>
        /// Get single element from entity collection by id
        /// </summary>
        /// <param name="id">Identifier of element</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Dto of element</returns>
        /// <exception cref="KeyNotFoundException">Occurs when element is not presented in collection</exception>
        public virtual async Task<TDto> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            var item = await GetEntityByIdAsync(id, token);
            return Mapper.Map<TDto>(item);
        }

        /// <summary>
        /// Get single element from entity collection by id
        /// </summary>
        /// <param name="id">Identifier of element</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Entity element</returns>
        /// <exception cref="KeyNotFoundException">Occurs when element is not presented in collection</exception>
        protected async Task<T> GetEntityByIdAsync(Guid id, CancellationToken token = default)
        {
            var item = await Repository.GetHashAsync(id, token);
            if (item != default) return item;

            var msg = $"Requested {EntityName} with id: {id} was not found in catalog.";
            Logger.LogWarning(msg);
            throw new KeyNotFoundException(msg);
        }

        /// <summary>
        /// Append item to entity collection
        /// </summary>
        /// <param name="item">Item to append</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Dto of added item</returns>
        public virtual async Task<TDto> AddAsync(TRequestDto item, CancellationToken token = default)
        {
            try
            {
                var itemToInsert = Mapper.Map<T>(item);
                var insertedItem = await Repository.CreateAsync(itemToInsert, token);
                return Mapper.Map<TDto>(insertedItem);
            }
            catch (OperationCanceledException)
            {
                // Throw cancelled operation, do not catch
                throw;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception during attempt to insert record of {EntityName}");
                throw;
            }
        }

        /// <summary>
        /// Update element of entity collection by id
        /// </summary>
        /// <param name="id">Identifier of element</param>
        /// <param name="item">New value</param>
        /// <param name="token">Token for operation cancel</param>
        /// <exception cref="KeyNotFoundException">Occurs when element is not presented in collection</exception>
        public virtual async Task UpdateAsync(Guid id, TRequestDto item, CancellationToken token = default)
        {
            var itemFromDb = await Repository.GetAsync(id, token);

            if (itemFromDb == null)
            {
                Logger.LogWarning($"Requested {EntityName} for update with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            var itemUpdated = Mapper.Map<T>(item);
            await Repository.UpdateAsync(id, itemUpdated, token);
        }

        /// <summary>
        /// Delete element from entity collection by id
        /// </summary>
        /// <param name="id">Identifier of element</param>
        /// <param name="token">Token for operation cancel</param>
        /// <returns>Deletion result</returns>
        /// <exception cref="KeyNotFoundException">Occurs when element is not presented in collection</exception>
        public virtual async Task<T> DeleteAsync(Guid id, CancellationToken token = default)
        {
            var item = await Repository.GetAsync(id, token);

            if (item == null)
            {
                Logger.LogWarning($"Requested {EntityName} for delete with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            await Repository.RemoveAsync(item, token);
            return item;
        }

        /// <summary>
        /// Delete multiple elements from entity collection by id
        /// </summary>
        /// <param name="token">Token for operation cancel</param>
        /// <param name="ids">Array of identifiers</param>
        public virtual async Task DeleteAsync(Guid[] ids, CancellationToken token = default)
        {
            if (ids.Length > 0) await Repository.RemoveManyAsync(ids, token);
        }
    }
}