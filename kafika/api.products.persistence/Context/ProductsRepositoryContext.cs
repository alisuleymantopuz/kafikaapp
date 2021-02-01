using api.products.persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.products.persistence.Context
{
    public class ProductsRepositoryContext : DbContext
    {
        public ProductsRepositoryContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
