using api.products.persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api.products.persistence.Extensions
{
    public static  class PersistenceExtensions
    {
        public static void RegisterProductsDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            DbContext(services, configuration);
        }

        private static void DbContext(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ProductsDefaultConnection");
            services.AddDbContext<ProductsRepositoryContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
