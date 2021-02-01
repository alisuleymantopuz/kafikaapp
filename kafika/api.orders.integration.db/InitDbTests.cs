using api.orders.persistence.Context;
using api.orders.persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace api.orders.integration.db
{
    public class InitDbTests
    {
        private readonly IConfigurationRoot _configuration;

        private readonly DbContextOptions _ordersDbOptions;

        private readonly Random _random;

        public InitDbTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();

            string ordersConnectionString = _configuration.GetConnectionString("OrdersDefaultConnection");

            _ordersDbOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString: ordersConnectionString).Options;

            _random = new Random();
        }


        [Fact]
        public async Task InitDbWithFakeData_FromScratch()
        {
            var ordersRepositoryContext = new OrdersRepositoryContext(_ordersDbOptions);
            ordersRepositoryContext.Database.EnsureCreated();
            ordersRepositoryContext.Users.RemoveRange(ordersRepositoryContext.Users);
            ordersRepositoryContext.Orders.RemoveRange(ordersRepositoryContext.Orders);
            ordersRepositoryContext.OrderDetails.RemoveRange(ordersRepositoryContext.OrderDetails);
            ordersRepositoryContext.SaveChanges();

            if (!ordersRepositoryContext.Users.Any(x => x.Username == "atopuz"))
            {
                ordersRepositoryContext.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "ali",
                    LastName = "topuz",
                    Username = "atopuz",
                    Password = "1"
                });

                await ordersRepositoryContext.SaveChangesAsync();
            }

            Assert.True(true);
        }
    }
}
