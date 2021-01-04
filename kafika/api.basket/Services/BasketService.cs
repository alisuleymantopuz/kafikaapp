using api.basket.Configuration;
using api.basket.Models;
using MongoDB.Driver;
using System.Collections.Generic;
namespace api.basket.Services
{
    public class BasketService
    {
        private readonly IMongoCollection<Basket> _baskets;

        public BasketService(IBasketStorageSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _baskets = database.GetCollection<Basket>(settings.BasketCollectionName);
        }

        public List<Basket> Get() => _baskets.Find(Basket => true).ToList();

        public Basket Get(string id) => _baskets.Find<Basket>(Basket => Basket.Id == id).FirstOrDefault();
        public Basket GetByBasketKey(string basketKey) => _baskets.Find<Basket>(Basket => Basket.BasketKey== basketKey).FirstOrDefault();

        public Basket Create(Basket Basket)
        {
            _baskets.InsertOne(Basket);
            return Basket;
        }

        public void Update(string basketKey, Basket BasketIn) => _baskets.ReplaceOne(Basket => Basket.BasketKey == basketKey, BasketIn);

        public void Remove(Basket BasketIn) => _baskets.DeleteOne(Basket => Basket.Id == BasketIn.Id);

        public void Remove(string basketKey) => _baskets.DeleteOne(Basket => Basket.BasketKey == basketKey);
    }
}
