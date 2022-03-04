using Catalog.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Catalog;

/// <summary>
///     Main class of application
/// </summary>
public static class Program
{
    /// <summary>
    ///     Entrypoint of application
    /// </summary>
    /// <param name="args">Launch arguments</param>
    public static void Main(string[] args)
    {
        // Configure logging at first
        LoggingHelper.ConfigureLogging();
        CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    ///     Create HostBuilder for application
    /// </summary>
    /// <param name="args">Array of arguments</param>
    /// <returns>
    ///     <see cref="IHostBuilder" />
    /// </returns>
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001", "http://192.168.1.108:5002",
                    "https://192.168.1.108:5003");
                webBuilder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http1);
                    options.ListenAnyIP(5001, o =>
                    {
                        o.Protocols = HttpProtocols.Http1;
                        o.UseHttps();
                    });
                    options.ListenAnyIP(5002, o => o.Protocols = HttpProtocols.Http1);
                    options.ListenAnyIP(5003, o =>
                    {
                        o.Protocols = HttpProtocols.Http1;
                        o.UseHttps();
                    });
                    options.ListenAnyIP(5004, o => o.Protocols = HttpProtocols.Http2);
                });
            })
            .UseSerilog();
    }
}