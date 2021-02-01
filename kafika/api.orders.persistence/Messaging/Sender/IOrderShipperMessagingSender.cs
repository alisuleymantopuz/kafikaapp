using api.orders.persistence.Entities;

namespace api.orders.persistence.Messaging.Sender
{
    public interface IOrderShipperMessagingSender
    {
        void SendShipperAssignedOrder(Order order);
    }
}
