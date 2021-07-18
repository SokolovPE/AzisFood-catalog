using Catalog.DataAccess.Models;
using Catalog.Dto;
using Catalog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Controllers
{
    public class IngredientController : BaseController<Ingredient, IngredientDto, IngredientRequestDto>
    {
        public IngredientController(ILogger<IngredientController> logger,
            IService<Ingredient, IngredientDto, IngredientRequestDto> service) : base(logger,
            service)
        {
        }
    }
}