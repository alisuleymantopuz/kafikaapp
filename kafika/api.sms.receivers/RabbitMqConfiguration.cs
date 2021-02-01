namespace api.sms.receivers
{
    public class RabbitMqConfiguration
    {
        public string Hostname { get; set; }
        public string OrderQueueName { get; set; }
        public string OrderProductsQueueName { get; set; }
        public string ShippingQueueName { get; set; }
        public string DeliveryQueueName { get; set; }
        public string SMSQueueName { get; set; }
        public string EmailQueueName { get; set; }
        public string CancelledQueueName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
    }
}
