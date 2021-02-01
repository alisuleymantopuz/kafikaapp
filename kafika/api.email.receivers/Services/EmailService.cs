using api.orders.persistence.Context;
using api.orders.persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace api.email.receivers
{
    public class EmailService : IEmailService
    {
        private readonly IServiceProvider _serviceProvider;
        public EmailService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SendEmail(Guid orderId, string to, string from, string topic, string message, string notes = "")
        {
            //just to see interaction!
            System.Threading.Thread.Sleep(10000);

            using (var scope = _serviceProvider.CreateScope())
            {
                var orderContext = scope.ServiceProvider.GetService<OrdersRepositoryContext>();
                var order = orderContext.Orders.Include(x => x.OrderDetails).FirstOrDefault(x => x.Id == orderId);
                if (order != null)
                {
                    //generate dummy email!
                    var client = new SmtpClient("smtp.mailtrap.io", 2525)
                    {
                        Credentials = new NetworkCredential("e747........6d", "0921.......f4f"),
                        EnableSsl = true
                    };

                    var sb = new StringBuilder();
                    sb.AppendLine(message);
                    sb.AppendLine(order.OrderNotes);

                    client.Send(from, to, topic, sb.ToString());

                    if (!string.IsNullOrEmpty(notes))
                        new StringBuilder().AppendLine(order.OrderNotes).AppendLine(notes).ToString();

                    order.LastModifiedDate = DateTime.UtcNow;
                    orderContext.SaveChanges();
                }
            }
        }
    }
}
