using Infrastructure.MQ;
using Infrastructure.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQDemo.Processor
{
    public class RabbitMQCustomer : IRabbitMQCustomer
    {
        private readonly MQConfig _mQConfig;

        public RabbitMQCustomer(IOptions<MQConfig> mQConfig)
        {
            _mQConfig = mQConfig.Value;
        }

        public string CreateCustommerInstance()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = _mQConfig.hostname,
                UserName = _mQConfig.username,
                Password = _mQConfig.password
            };

            string msg = string.Empty;
            try
            {
                ConsumerOralScanPacket(connectionFactory);
            }
            catch (Exception ex)
            {
                msg = $"ex=>{ex.Message}";
                //LogHelper.Error($"MQCustomer ex={connectionFactory.HostName}|{connectionFactory.UserName}|{connectionFactory.Password}{msg}");
            }
            return msg;
        }

        /// <summary>
        /// 解析口扫数据包
        /// </summary>
        /// <param name="connectionFactory"></param>
        private void ConsumerOralScanPacket(ConnectionFactory connectionFactory)
        {
            //创建连接
            var _connection = connectionFactory.CreateConnection("UltraHub QueueProcessor Connection");
            //创建通道
            var _model = _connection.CreateModel();
            //限制RabbitMQ，使得每个Consumer在同一时间点最多处理一个Message
            _model.BasicQos(0, 1, false);
            // 声明队列，持久化需要另外设置
            _model.QueueDeclare(DTKeys.MQ_OSP_QueueName, true, false, false, null);
            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(_model);
            //接收到消息事件
            consumer.Received += (model, ea) =>
            {
                string msg = string.Empty;
                var message = Encoding.UTF8.GetString(ea.Body);
                try
                {
                    //在这里调用消费方法
                    //_oralScanPacket.Resolve(message, out msg);
                    Console.WriteLine("消费了一条消息：" + message);
                }
                catch (Exception ex)
                {
                    msg = $"ex=>{ex.ToString()}";
                }

                //LogHelper.Error($"ConsumerOralScanPacket -> 收到【口扫数据包】处理消息： {message}，处理结果：{msg}");

                //确认该消息已被消费
                _model.BasicAck(ea.DeliveryTag, false);
            };

            _model.BasicConsume(DTKeys.MQ_OSP_QueueName, false, consumer); //需要接受方发送ack回执,删除消息
        }
    }
    public static class RabbitMQCustomerBuilder
    {
        public static IApplicationBuilder UseRabbitMQCustomer(this IApplicationBuilder app, IRabbitMQCustomer rabbitMQCustomer)
        {
            rabbitMQCustomer.CreateCustommerInstance();
            return app;
        }
    }
}
