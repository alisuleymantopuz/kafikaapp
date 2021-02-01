using api.orders.persistence.Entities;
using api.orders.persistence.Messaging.Sender;
using api.orders.persistence.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace api.orders.receivers.delivery
{
    //BasicAck
    public class DeliveryMessagingReceiver : IHostedService
    {
        private IModel _channel;
        private IConnection _connection;
        private readonly IDeliveryService _deliveryService;
        private readonly IEmailMessagingSender _emailMessagingSender;
        private readonly ISmsMessagingSender _smsMessagingSender;
        private readonly string _hostname;
        private readonly string _queueName;


        public DeliveryMessagingReceiver(IDeliveryService deliveryService, IEmailMessagingSender emailMessagingSender, ISmsMessagingSender smsMessagingSender, IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.DeliveryQueueName;
            _deliveryService = deliveryService;
            _smsMessagingSender = smsMessagingSender;
            _emailMessagingSender = emailMessagingSender;
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_hostname)
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        //Initial status set!
        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var order = JsonConvert.DeserializeObject<Order>(content);
                HandleMessage(order);

                _channel.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private void HandleMessage(Order order)
        {
            _smsMessagingSender.SendSms(order);
            _emailMessagingSender.SendEmail(order);
            _deliveryService.InformForDelivery(order.Id, "Email and sms is sent!");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {

        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {

        }
    }
}
