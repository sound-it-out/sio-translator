using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SIO.Translator.Migrations.Extensions;
using System.Threading.Tasks;

namespace SIO.Translator.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.SeedDatabaseAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
