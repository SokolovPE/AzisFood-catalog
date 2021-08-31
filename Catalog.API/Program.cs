using Catalog.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Catalog
{
    /// <summary>
    /// Main class of application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entrypoint of application.
        /// </summary>
        /// <param name="args">Launch arguments.</param>
        public static void Main(string[] args)
        {
            // Configure logging at first.
            LoggingHelper.ConfigureLogging();
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Create HostBuilder for application.
        /// </summary>
        /// <param name="args">Array of arguments.</param>
        /// <returns><see cref="IHostBuilder"/>.</returns>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();
    }
}