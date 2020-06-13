using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SIO.Migrations.DbContexts;
using System.Threading.Tasks;

namespace SIO.Migrations.Extensions
{
    public static class HostExtensions
    {
        public static async Task<IHost> SeedDatabaseAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<SIOTranslatorDbContext>())
                    await context.Database.MigrateAsync();
            }

            return host;
        }
    }
}
