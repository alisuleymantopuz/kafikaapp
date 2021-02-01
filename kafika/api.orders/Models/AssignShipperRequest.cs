using System.ComponentModel.DataAnnotations;

namespace api.orders.Models
{
    public class AssignShipperRequest
    {

        [Required]
        public string OrderId { get; set; }
        
        [Required]
        public string ShipperPhone { get; set; }
        
        [Required]
        public string ShipperName { get; set; }

        [Required]
        public string ShippingAddress { get; set; }
    }

    public class AddDeliveryNote
    {

        [Required]
        public string OrderId { get; set; }

        [Required]
        public string Notes { get; set; } 
    }

    public class CancelOrder
    {

        [Required]
        public string OrderId { get; set; } 
    }
}
