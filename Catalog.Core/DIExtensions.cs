using Catalog.Core.Mappings;
using Catalog.Core.Services.Implementations;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Extensions
{
    /// <summary>
    /// Extensions for using AutoMapper
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// Add automapper to application
        /// </summary>
        /// <param name="serviceCollection">Collection of services</param>
        public static void AddMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DefaultMappingProfile>();
            });
        }

        /// <summary>
        /// Add main services to application
        /// </summary>
        /// <param name="services">Collection of services</param>
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IService<Ingredient, IngredientDto, IngredientRequestDto>),
                typeof(BaseService<Ingredient, IngredientDto, IngredientRequestDto>));
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IValidatorService<Product>, ProductValidatorService>();
        }
    }
}