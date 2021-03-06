using AutoMapper;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;

namespace Catalog.Core.Mappings;

/// <summary>
///     Default mapping profile
/// </summary>
public class DefaultMappingProfile : Profile
{
    /// <summary>
    ///     Basic mapping profle
    /// </summary>
    public DefaultMappingProfile()
    {
        #region Product

        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<Product, ProductRequestDto>().ReverseMap();
        CreateMap<ProductDto, ProductRequestDto>().ReverseMap();

        #endregion

        #region Ingredient

        CreateMap<Ingredient, IngredientDto>().ReverseMap();
        CreateMap<Ingredient, IngredientRequestDto>().ReverseMap();
        CreateMap<IngredientDto, IngredientRequestDto>().ReverseMap();

        #endregion

        #region Option

        CreateMap<Option, OptionDto>().ReverseMap();
        CreateMap<Option, OptionRequestDto>().ReverseMap();
        CreateMap<OptionDto, OptionRequestDto>().ReverseMap();

        #endregion

        #region Category

        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryRequestDto>().ReverseMap();
        CreateMap<CategoryDto, CategoryRequestDto>().ReverseMap();

        #endregion

        #region Others

        CreateMap<NutritionFact, NutritionFactDto>().ReverseMap();
        CreateMap<IngredientUsage, IngredientUsageDto>().ReverseMap();

        #endregion
    }
}