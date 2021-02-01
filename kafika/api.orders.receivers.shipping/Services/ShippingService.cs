using api.orders.persistence.Context;
using api.orders.persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq; 
using System.Text;

namespace api.orders.receivers.shipping
{
    public class ShippingService : IShippingService
    {
        private readonly IServiceProvider _serviceProvider;
        public ShippingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void InformShipper(Guid orderId, string notes = "")
        {
            //just to see interaction!
            System.Threading.Thread.Sleep(10000);


            //just to see interaction!
            System.Threading.Thread.Sleep(10000);

            using (var scope = _serviceProvider.CreateScope())
            {
                var orderContext = scope.ServiceProvider.GetService<OrdersRepositoryContext>();
                var order = orderContext.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.Id == orderId);
                if (order != null)
                {
                    if (!string.IsNullOrEmpty(notes))
                        new StringBuilder().AppendLine(order.OrderNotes).AppendLine(notes).ToString();

                    order.LastModifiedDate = DateTime.UtcNow;
                    orderContext.SaveChanges();
                }
            }
        }
    }
}
