using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace Infrastructure.MQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private IConnection _connection;
        private readonly MQConfig _mQConfig;

        public RabbitMQProducer(IOptions<MQConfig> mQConfig)
        {
            _mQConfig = mQConfig.Value;

            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = _mQConfig.hostname,
                UserName = _mQConfig.username,
                Password = _mQConfig.password
            };
            try
            {
                //创建连接
                _connection = connectionFactory.CreateConnection("UltraHub QueueProcessor Connection");
            }
            catch (Exception ex)
            {
                //LogHelper.Error($"MQProducer ex=>{connectionFactory.HostName}|{connectionFactory.UserName}|{connectionFactory.Password}{ex.Message}");
            }
        }

        /// <summary>
        /// DirectExchange（一般为默认）
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="queueName"></param>
        /// <param name="routeKey"></param>
        /// <param name="message"></param>
        public void ProducerDirectExchange(string exchangeName, string queueName, string routeKey, string message)
        {
            //创建通道
            var channel = _connection.CreateModel();
            //定义一个Direct类型交换机,durable(可持久化)
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
            //定义一个队列,durable（可持久化）
            channel.QueueDeclare(queueName, true, false, false, null);
            //将队列绑定到交换机
            channel.QueueBind(queueName, exchangeName, routeKey, null);
            //发布消息
            var sendBytes = Encoding.UTF8.GetBytes(message);
            //消息可持久化
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;//可持久化
            //properties.Persistent = true;//需要持久化Message，即在Publish的时候指定一个properties，
            channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);

            //关闭
            channel.Close();
        }
    }
}
