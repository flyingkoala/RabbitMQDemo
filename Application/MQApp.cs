using Infrastructure.MQ;
using Infrastructure.Setting;
using System;

namespace Application
{

    public class MQApp
    {
        //private static HeyGearsConfig _config;
        //private readonly IRedis _redis;
        private readonly IRabbitMQProducer _mqProducer;

        public MQApp(IRabbitMQProducer mqProducer)//HeyGearsConfigService service, IRedis redis, 
        {
            //_config = service.config;
            //_redis = redis;
            _mqProducer = mqProducer;
        }

        /// <summary>
        /// 发布消息到消息队列(口扫包)
        /// </summary>
        /// <param name="objReq"></param>
        /// <returns></returns>
        public void PublishMessage(string message)
        {
            
            try
            {
               
                _mqProducer.ProducerDirectExchange(DTKeys.MQ_OSP_ExchangeName, DTKeys.MQ_OSP_QueueName, DTKeys.MQ_OSP_RouteKey, message);
                Console.WriteLine("生产了一条消息：" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("生产消息出错");
            }

            
        }
    }
}
