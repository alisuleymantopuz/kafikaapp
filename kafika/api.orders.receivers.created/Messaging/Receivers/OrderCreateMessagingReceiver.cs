﻿using api.orders.persistence.Entities;
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

namespace api.orders.receivers.created
{
    //BasicAck
    public class OrderCreateMessagingReceiver : IHostedService
    {
        private IModel _channel;
        private IConnection _connection;
        private readonly IOrderStatusUpdateService _orderStatusUpdateService;
        private readonly IOrderProductsUpdateMessagingSender _orderProductsUpdateMessagingSender;
        private readonly IEmailMessagingSender _emailMessagingSender;
        private readonly ISmsMessagingSender _smsMessagingSender;
        private readonly IRevertProductStocksMessagingSender _revertProductStocksMessagingSender;
        private readonly string _hostname;
        private readonly string _queueName;


        public OrderCreateMessagingReceiver(IOrderStatusUpdateService orderStatusUpdateService, IOrderProductsUpdateMessagingSender orderProductsUpdateMessagingSender, IRevertProductStocksMessagingSender revertProductStocksMessagingSender, IEmailMessagingSender emailMessagingSender, ISmsMessagingSender smsMessagingSender, IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.OrderQueueName;
            _orderStatusUpdateService = orderStatusUpdateService;
            _orderProductsUpdateMessagingSender = orderProductsUpdateMessagingSender;
            _revertProductStocksMessagingSender = revertProductStocksMessagingSender;
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
            switch (order.OrderStatus)
            {
                case OrderStatus.Created:
                    {

                        _orderStatusUpdateService.UpdateStatus(order.Id, OrderStatus.InProgress, "Order created and it is in progress");
                        _orderProductsUpdateMessagingSender.SendCreatedOrder(order);
                        _emailMessagingSender.SendEmail(order);
                        _smsMessagingSender.SendSms(order);
                    }
                    break;   
                case OrderStatus.CancelRequested:
                    {
                        _orderStatusUpdateService.UpdateStatus(order.Id, OrderStatus.Cancelled, "Order is cancelled!");
                        _revertProductStocksMessagingSender.UpdateStocks(order);
                        _emailMessagingSender.SendEmail(order); 
                    }
                    break;
                default:
                    break;
            }
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
