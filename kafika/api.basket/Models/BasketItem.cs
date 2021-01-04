using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace api.basket.Models
{
    public class BasketItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required] 
        public string ProductId { get; set; }

        [Required]
        [BsonElement("Quantity")] 
        public int Quantity { get; set; }

        [Required]
        [BsonElement("AddedDate")]
        public DateTime AddedDate { get; set; }
    }
}
