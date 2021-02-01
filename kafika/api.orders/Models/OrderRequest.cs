using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.orders.Models
{
    public class OrderRequest
    {

        [Required]
        public string FullAddress { get; set; }

        [Required]
        public List<OrderDetailModel> OrderDetails { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string OrderNotes { get; set; } //just for an example
    }
}
