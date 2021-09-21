using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    /// <summary>
    /// Controller to operate ingredients
    /// </summary>
    public class IngredientController : BaseController<Ingredient, IngredientDto, IngredientRequestDto>
    {
        /// <inheritdoc />
        public IngredientController(ILogger<IngredientController> controllerLogger,
            IService<Ingredient, IngredientDto, IngredientRequestDto> service) : base(controllerLogger,
            service)
        {
        }
    }
}