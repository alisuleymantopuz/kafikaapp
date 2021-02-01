using System;
using System.Collections.Generic;
using System.Linq;

namespace api.orders.persistence.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus OrderStatus { get; set; } //just for an example
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public User User { get; set; }
        public string OrderNotes { get; set; } //just for an example 
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string ShipperPhone { get; set; }
        public string ShipperName { get; set; }
        public string ShippingAddress { get; set; }


        public void GenerateNewId()
        {
            Id = Guid.NewGuid();

            if (OrderDetails != null && OrderDetails.Any())
            {
                OrderDetails.ForEach(x =>
                {
                    x.Id = Guid.NewGuid();
                });
            }
        }
    }
}
