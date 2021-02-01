using System;

namespace api.orders.persistence.Services
{
    public interface IDeliveryService
    {
        void InformForDelivery(Guid orderId, string notes = "");
    }
}
