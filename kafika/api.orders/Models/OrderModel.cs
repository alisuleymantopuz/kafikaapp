using api.orders.persistence.Entities;
using System;
using System.Collections.Generic;

namespace api.orders.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; } 
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; } 
        public string OrderNotes { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string ShipperPhone { get; set; }
        public string ShipperName { get; set; }
        public string ShippingAddress { get; set; }
    }
}
