using domain;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.ef
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(t => new { t.Id });
            modelBuilder.Entity<Product>().Property(u => u.Id).IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.Name).IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.UnitPrice).IsRequired();
            modelBuilder.Entity<Product>().Property(u => u.UnitsInStock).IsRequired();
        }
    }
}