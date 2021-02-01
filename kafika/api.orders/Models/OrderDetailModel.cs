using System;

namespace api.orders.Models
{
    public class OrderDetailModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string OrderNotes { get; set; }
    }
}
