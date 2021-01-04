namespace api.basket.Configuration
{
    public class BasketStorageSettings : IBasketStorageSettings
    {
        public string BasketCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBasketStorageSettings
    {
        string BasketCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
