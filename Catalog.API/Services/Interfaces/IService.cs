using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.DataAccess.Models;
using Catalog.Dto;

namespace Catalog.Services.Interfaces
{
    public interface IService<T, TDto, in TRequestDto> 
        where T: IRepoEntity 
        where TDto: IDto
        where TRequestDto: IRequestDto
    {
        /// <summary>
        /// Get all <see cref="T"/> presented in catalog.
        /// </summary>
        /// <returns>List of <see cref="T"/></returns>
        Task<List<TDto>> GetAllAsync();

        /// <summary>
        /// Get <see cref="T"/> from catalog by id.
        /// </summary>
        /// <param name="id">Identifier of <see cref="T"/>.</param>
        /// <returns>Instance of <see cref="TDto"/>.</returns>
        Task<TDto> GetByIdAsync(string id);

        /// <summary>
        /// Add new <see cref="T"/> to catalog.
        /// </summary>
        /// <param name="product"><see cref="T"/> to add.</param>
        /// <returns>Insertion result.</returns>
        Task<TDto> AddAsync(TRequestDto product);

        /// <summary>
        /// Update <see cref="T"/> in catalog.
        /// </summary>
        /// <param name="id">Identifier of <see cref="T"/> to be updated.</param>
        /// <param name="product"><see cref="T"/> to be updated.</param>
        /// <returns>Insertion result.</returns>
        Task UpdateAsync(string id, TRequestDto product);

        /// <summary>
        /// Delete <see cref="T"/> from catalog.
        /// </summary>
        /// <param name="id">Identifier of <see cref="T"/> to be deleted.</param>
        /// <returns>Deletion result.</returns>
        Task<T> DeleteAsync(string id);

        /// <summary>
        /// Delete <see cref="T"/> from catalog.
        /// </summary>
        /// <param name="ids">Array with identifiers of <see cref="T"/> to be deleted.</param>
        /// <returns>Deletion result.</returns>
        Task DeleteAsync(string[] ids);
        
    }
}