using Catalog.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Extensions
{
    /// <summary>
    /// Extensions for using AutoMapper.
    /// </summary>
    public static class MapperExtensions
    {
        public static void AddMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DefaultMappingProfile>();
            });
        }
    }
}