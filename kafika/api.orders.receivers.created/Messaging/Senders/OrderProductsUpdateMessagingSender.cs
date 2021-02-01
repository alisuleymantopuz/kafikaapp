using api.orders.persistence.Entities;
using api.orders.persistence.Messaging.Sender;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace api.orders.receivers.created
{
    public class OrderProductsUpdateMessagingSender : IOrderProductsUpdateMessagingSender
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private IConnection _connection;

        public OrderProductsUpdateMessagingSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _queueName = rabbitMqOptions.Value.OrderProductsQueueName;
            _hostname = rabbitMqOptions.Value.Hostname;
            CreateConnection();
        }
        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_hostname)
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }

        public void SendCreatedOrder(Order order)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var json = JsonConvert.SerializeObject(order);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                }
            }
        }
    }
}
