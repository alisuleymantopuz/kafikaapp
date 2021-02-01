using System;

namespace api.orders.persistence.Entities
{
    public class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string OrderNotes { get; set; } //just for an example
    }
}
