using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.basket.Models
{
    public class Basket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        [BsonElement("BasketKey")]
        public string BasketKey { get; set; }

        [Required]
        [BsonElement("LastUpdatedDate")]
        public DateTime LastUpdatedDate { get; set; }

        [BsonElement("Items")]
        public IEnumerable<BasketItem> Items { get; set; }

    }
}
