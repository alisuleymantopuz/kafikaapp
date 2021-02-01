using api.orders.persistence.Entities;
using System;

namespace api.orders.persistence.Services
{
    public interface IOrderProductsUpdateService
    {
        void UpdateProducts(Guid orderId, string notes = "");
    }
}
