using api.products.persistence.Context;
using api.products.persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace api.products.integration.db
{
    public class InitDbTests
    {

        public const string ImportURL = "https://random-data-api.com/api/commerce/random_commerce?size=100";

        private readonly IConfigurationRoot _configuration;

        private readonly DbContextOptions _productsDbOptions;

        private readonly Random _random;


        public InitDbTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = builder.Build();

            string productsConnectionString = _configuration.GetConnectionString("ProductsDefaultConnection");

            _productsDbOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString: productsConnectionString).Options;

            _random = new Random();
        }


        [Fact]
        public async Task Init()
        {
            var productsRepositoryContext = new ProductsRepositoryContext(_productsDbOptions);
            productsRepositoryContext.Database.EnsureCreated();
            productsRepositoryContext.Products.RemoveRange(productsRepositoryContext.Products);


            var baseUrl = ImportURL;
            var imported = new List<ProductImport>();

            using (HttpClient client = new HttpClient())
            {
                using HttpResponseMessage res = await client.GetAsync(baseUrl);
                using HttpContent content = res.Content;
                var data = await content.ReadAsStringAsync();
                imported.AddRange(JsonConvert.DeserializeObject<List<ProductImport>>(data));
            }

            imported.ForEach(async (x) =>
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = x.ProductName,
                    UnitPrice = x.Price,
                    UnitsInStock = _random.Next(10, 99)
                };
                await productsRepositoryContext.Products.AddAsync(product);
            });

            await productsRepositoryContext.SaveChangesAsync();

            Assert.True(true);
        }
    }
}
