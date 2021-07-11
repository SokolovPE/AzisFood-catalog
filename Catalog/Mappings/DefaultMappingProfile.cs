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
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductRequestDto>().ReverseMap();
            CreateMap<ProductDto, ProductRequestDto>().ReverseMap();
        }
    }
}