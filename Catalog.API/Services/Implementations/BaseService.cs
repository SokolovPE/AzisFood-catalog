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
    public class BaseService<T, TDto, TRequestDto> : IService<T, TDto, TRequestDto>
        where T: IRepoEntity 
        where TDto: IDto
        where TRequestDto: IRequestDto
    {
        protected readonly ILogger<BaseService<T, TDto, TRequestDto>> Logger;
        protected readonly IMapper Mapper;
        protected readonly ICachedBaseRepository<T> Repository;
        protected readonly string EntityName;

        public BaseService(ILogger<BaseService<T, TDto, TRequestDto>> logger,
            IMapper mapper,
            ICachedBaseRepository<T> repository)
        {
            Logger = logger;
            Mapper = mapper;
            Repository = repository;
            EntityName = typeof(T).Name;
        }
        
        public virtual async Task<List<TDto>> GetAllAsync()
        {
            try
            {
                var items = await Repository.GetAsync();
                return Mapper.Map<List<TDto>>(items);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception during attempt to get all {EntityName} from catalog");
                throw;
            }
        }

        public virtual async Task<TDto> GetByIdAsync(string id)
        {
            var item = await Repository.GetAsync(id);

            if (item != null) return Mapper.Map<TDto>(item);
            
            var msg = $"Requested {EntityName} with id: {id} was not found in catalog.";
            Logger.LogWarning(msg);
            throw new KeyNotFoundException(msg);
        }

        public virtual async Task<TDto> AddAsync(TRequestDto item)
        {
            try
            {
                var itemToInsert = Mapper.Map<T>(item);
                var insertedItem = await Repository.CreateAsync(itemToInsert);
                return Mapper.Map<TDto>(insertedItem);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Exception during attempt to insert record of {EntityName}");
                throw;
            }
        }

        public virtual async Task UpdateAsync(string id, TRequestDto item)
        {
            var itemFromDb = await Repository.GetAsync(id);

            if (itemFromDb == null)
            {
                Logger.LogWarning($"Requested {EntityName} for update with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            var itemUpdated = Mapper.Map<T>(item);
            itemUpdated.Id = id;
            await Repository.UpdateAsync(id, itemUpdated);
        }

        public virtual async Task<T> DeleteAsync(string id)
        {
            var item = await Repository.GetAsync(id);

            if (item == null)
            {
                Logger.LogWarning($"Requested {EntityName} for delete with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            await Repository.RemoveAsync(item.Id);
            return item;
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            if (ids.Length > 0) await Repository.RemoveManyAsync(ids);
        }
    }
}