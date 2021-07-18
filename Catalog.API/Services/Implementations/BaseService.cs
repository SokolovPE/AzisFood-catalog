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
        private readonly ILogger<BaseService<T, TDto, TRequestDto>> _logger;
        private readonly IMapper _mapper;
        private readonly ICachedBaseRepository<T> _repository;
        private readonly string _entityName;

        public BaseService(ILogger<BaseService<T, TDto, TRequestDto>> logger,
            IMapper mapper,
            ICachedBaseRepository<T> repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _entityName = typeof(T).Name;
        }
        
        public virtual async Task<List<TDto>> GetAllAsync()
        {
            try
            {
                var items = await _repository.GetAsync();
                return _mapper.Map<List<TDto>>(items);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception during attempt to get all {_entityName} from catalog");
                throw;
            }
        }

        public virtual async Task<TDto> GetByIdAsync(string id)
        {
            var item = await _repository.GetAsync(id);

            if (item != null) return _mapper.Map<TDto>(item);
            
            var msg = $"Requested {_entityName} with id: {id} was not found in catalog.";
            _logger.LogWarning(msg);
            throw new KeyNotFoundException(msg);
        }

        public virtual async Task<TDto> AddAsync(TRequestDto item)
        {
            try
            {
                var itemToInsert = _mapper.Map<T>(item);
                var insertedItem = await _repository.CreateAsync(itemToInsert);
                return _mapper.Map<TDto>(insertedItem);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception during attempt to insert record of {_entityName}");
                throw;
            }
        }

        public virtual async Task UpdateAsync(string id, TRequestDto item)
        {
            var itemFromDb = await _repository.GetAsync(id);

            if (itemFromDb == null)
            {
                _logger.LogWarning($"Requested {_entityName} for update with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            var itemUpdated = _mapper.Map<T>(item);
            itemUpdated.Id = id;
            await _repository.UpdateAsync(id, itemUpdated);
        }

        public virtual async Task<T> DeleteAsync(string id)
        {
            var item = await _repository.GetAsync(id);

            if (item == null)
            {
                _logger.LogWarning($"Requested {_entityName} for delete with id: {id} was not found in catalog.");
                throw new KeyNotFoundException();
            }

            await _repository.RemoveAsync(item.Id);
            return item;
        }

        public virtual async Task DeleteAsync(string[] ids)
        {
            if (ids.Length > 0) await _repository.RemoveManyAsync(ids);
        }
    }
}