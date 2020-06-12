using Microsoft.EntityFrameworkCore;
using SIO.Translator.Migrations.Entities;

namespace SIO.Translator.Migrations.DbContexts
{
    public class SIOTranslatorDbContext : DbContext
    {
        public DbSet<TranslatorState> TranslatorStates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TranslatorState>()
                .ToTable(nameof(TranslatorState))
                .HasKey(ps => ps.Name);

            base.OnModelCreating(builder);
        }
        public SIOTranslatorDbContext(DbContextOptions<SIOTranslatorDbContext> options) : base(options)
        {

        }

        public SIOTranslatorDbContext()
        {
        }
    }
}
