using Newtonsoft.Json;

namespace domain.Import
{
    public class ProductImport
    {
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}
