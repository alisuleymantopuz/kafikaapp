using System;

namespace api.orders.persistence.Services
{
    public interface IRevertProductStocksService
    {
        void RevertProductStocks(Guid orderId, string notes = "");
    }
}
