using System;
using System.ComponentModel.DataAnnotations;
namespace api.products.persistence.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(60, ErrorMessage = "Name can't be longer than 60 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Units in stock is required")]
        public int UnitsInStock { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
