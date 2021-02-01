using api.orders.persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api.orders.persistence.Extensions
{
    public static class PersistenceExtensions
    {
        public static void RegisterOrdersDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            DbContext(services, configuration);
        }

        private static void DbContext(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("OrdersDefaultConnection");
            services.AddDbContext<OrdersRepositoryContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
