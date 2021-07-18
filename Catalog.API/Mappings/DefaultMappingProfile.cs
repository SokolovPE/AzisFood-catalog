using AutoMapper;
using Catalog.DataAccess.Models;
using Catalog.Dto;

namespace Catalog.Mappings
{
    /// <summary>
    /// Default mapping profile.
    /// </summary>
    public class DefaultMappingProfile : Profile
    {
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
        }
    }
}