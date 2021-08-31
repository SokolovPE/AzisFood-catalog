using Catalog.DataAccess.Models;
using Catalog.Dto;
using Catalog.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    /// <summary>
    /// Controller to operate ingredients.
    /// </summary>
    public class IngredientController : BaseController<Ingredient, IngredientDto, IngredientRequestDto>
    {
        /// <inheritdoc />
        public IngredientController(ILogger<IngredientController> logger,
            IService<Ingredient, IngredientDto, IngredientRequestDto> service) : base(logger,
            service)
        {
        }
    }
}