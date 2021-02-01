using api.orders.persistence.Entities;
using System;

namespace api.orders.persistence.Services
{
    public interface IOrderStatusUpdateService
    {
        void UpdateStatus(Guid orderId, OrderStatus newStatus, string notes = ""); 
    }
}
