using System.IO;
using AzisFood.CacheService.Redis.Extensions;
using AzisFood.DataEngine.Cache.CacheService.Extensions;
using AzisFood.DataEngine.Core;
using AzisFood.DataEngine.Mongo.Extensions;
using AzisFood.MQ.Rabbit.Extensions;
using Catalog.Core;
using Catalog.Extensions;
using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Catalog
{
    /// <summary>
    /// Startup of application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructs startup
        /// </summary>
        /// <param name="configuration">Configuration of application</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container
        /// </summary>
        /// <param name="services">Collection of services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o =>
            {
                o.AddPolicy("_myAllowSpecificOrigins", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            
            services.AddControllers();
            
            // Register mappings
            services.AddMapper();
            
            // Add open tracing with Jaeger
            services.AddJaeger(Configuration);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Catalog.API", Version = "v1"});
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Catalog.API.xml");
                c.IncludeXmlComments(filePath);
            });
            
            // Add RabbitMQ MassTransit
            services.AddRabbitMQSupport(Configuration);
            
            // Add MongoDb config
            services
                .AddMongoSupport(Configuration)
                .UseCacheServiceAdapter();
            
            // Add Redis config
            services.AddRedisSupport(Configuration);
            
            // Registrations
            services.AddCoreServices();
            
            // Add GraphQL
            services.AddGraphQL();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        /// <summary>
        /// Configure services
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL();
            });
            
            app.UseGraphQLVoyager(new VoyagerOptions
            {
                GraphQLEndPoint = "/graphql"
            }, "/graphql-voyager");
        }
    }
}