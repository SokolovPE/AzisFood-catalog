using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    /// <summary>
    /// Controller to operate categories
    /// </summary>
    public class CategoryController : BaseController<Category, CategoryDto, CategoryRequestDto>
    {
        /// <inheritdoc />
        public CategoryController(ILogger<BaseController<Category, CategoryDto, CategoryRequestDto>> logger,
            IService<Category, CategoryDto, CategoryRequestDto> service) : base(logger, service)
        {
        }
    }
}