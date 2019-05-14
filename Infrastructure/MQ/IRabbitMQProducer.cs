namespace Infrastructure.MQ
{
    public interface IRabbitMQProducer
    {
        void ProducerDirectExchange(string exchangeName, string queueName, string routeKey, string message);
    }
}