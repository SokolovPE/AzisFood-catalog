using Catalog.Core.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Extensions
{
    /// <summary>
    /// Extensions for using AutoMapper.
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
    }
}