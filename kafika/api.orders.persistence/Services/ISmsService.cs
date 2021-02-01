using System;

namespace api.orders.persistence.Services
{
    public interface ISmsService
    {
        void SendSms(Guid orderId, string notes = "");
    }
}
