using System;

namespace api.orders.persistence.Services
{
    public interface IShippingService
    {
        void InformShipper(Guid orderId, string notes = "");
    }
}
