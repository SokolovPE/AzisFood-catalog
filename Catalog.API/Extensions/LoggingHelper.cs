using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Catalog.Extensions;

/// <summary>
///     Helper with logging
/// </summary>
public static class LoggingHelper
{
    /// <summary>
    ///     Configure logging in application
    /// </summary>
    public static void ConfigureLogging()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                true)
            .Build();
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
        {
            AutoRegisterTemplate = true,
            IndexFormat =
                $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
        };
    }
}