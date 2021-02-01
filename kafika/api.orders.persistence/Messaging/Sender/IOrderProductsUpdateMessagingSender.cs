using api.orders.persistence.Entities;

namespace api.orders.persistence.Messaging.Sender
{
    public interface IOrderProductsUpdateMessagingSender
    {
        void SendCreatedOrder(Order order);
    }
}
