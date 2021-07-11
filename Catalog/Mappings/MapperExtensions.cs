using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Mappings
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