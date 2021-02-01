using api.orders.persistence.Entities;

namespace api.orders.persistence.Messaging.Sender
{
    public interface IRevertProductStocksMessagingSender
    {
        void UpdateStocks(Order order);
    }

    public interface ISmsMessagingSender
    {
        void SendSms(Order order);
    }

    public interface IEmailMessagingSender
    {
        void SendEmail(Order order);
    }
}
