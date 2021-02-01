using Newtonsoft.Json;

namespace api.products.integration.db
{
    public class ProductImport
    {
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
