using Microsoft.EntityFrameworkCore;
using SIO.Migrations.Entities;

namespace SIO.Migrations.DbContexts
{
    public class SIOTranslatorDbContext : DbContext
    {
        public DbSet<TranslatorState> TranslatorStates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

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
