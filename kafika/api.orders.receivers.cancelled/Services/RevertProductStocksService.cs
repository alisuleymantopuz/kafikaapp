using api.orders.persistence.Context; 
using api.orders.persistence.Services;
using api.products.persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;

namespace api.orders.receivers.cancelled
{
    public class RevertProductStocksService : IRevertProductStocksService
    {
        private readonly IServiceProvider _serviceProvider;
        public RevertProductStocksService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RevertProductStocks(Guid orderId, string notes = "")
        {
            //just to see interaction!
            System.Threading.Thread.Sleep(10000);

            using (var scope = _serviceProvider.CreateScope())
            {
                var orderContext = scope.ServiceProvider.GetService<OrdersRepositoryContext>();

                var productContext = scope.ServiceProvider.GetService<ProductsRepositoryContext>();

                var order = orderContext.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.Id == orderId);
                if (order != null)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = productContext.Products.FirstOrDefault(x => x.Id == detail.ProductId);
                        if (product != null)
                        {
                            product.UnitsInStock += detail.Quantity;
                            product.LastModifiedDate = DateTime.UtcNow;
                            
                            if (!string.IsNullOrEmpty(notes))
                                order.OrderNotes = new StringBuilder().AppendLine(order.OrderNotes).AppendLine(notes).ToString();

                            order.LastModifiedDate = DateTime.UtcNow;
                        }
                    }
                    productContext.SaveChanges();
                    orderContext.SaveChanges();
                }
            }
        } 
    }
}
