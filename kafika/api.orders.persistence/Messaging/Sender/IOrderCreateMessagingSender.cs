using api.orders.persistence.Entities;

namespace api.orders.persistence.Messaging.Sender
{
    public interface IOrderCreateMessagingSender
    {
        void SendCreatedOrder(Order order);
    }

    public interface IOrderDeliveryMessagingSender
    {
        void SendDeliveryOrder(Order order);
    }
}
