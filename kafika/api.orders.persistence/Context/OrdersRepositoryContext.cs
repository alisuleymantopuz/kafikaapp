using api.orders.persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.orders.persistence.Context
{
    public class OrdersRepositoryContext : DbContext
    {
        public OrdersRepositoryContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
