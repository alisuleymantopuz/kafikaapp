using System;

namespace api.orders.persistence.Services
{
    public interface IEmailService
    {
        void SendEmail(Guid orderId, string to, string from, string topic, string message, string notes = "");
    }
}
